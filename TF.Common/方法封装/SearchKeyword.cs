using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TF.Common
{
    public class SearchResult
    {
        public SearchResult(string keyword, int start, int end, string text)
        {
            Success = true;
            Keyword = keyword;
            End = end;
            Start = Math.Max(end + 1 - ((keyword.Length - 1) * 5 + keyword.Length), start);
            var length = End - Start + 1;
            if (keyword.Length == length) { SearchString = keyword; return; }

            var t = Format(text.Substring(Start, End - Start + 1));
            var index = length - 1;
            for (int i = keyword.Length - 1; i >= 0; i--)
            {
                char c = keyword[i], c2 = t[index--];
                while (c != c2) { c2 = t[index--]; if (index == -1) break; }
            }
            index++;
            Start = Start + index;
            SearchString = text.Substring(Start, End - Start + 1);
        }
        public SearchResult(string keyword, int end)
        {
            Success = true;
            Keyword = keyword;
            SearchString = keyword;
            End = end;
            Start = End - Keyword.Length + 1;
        }

        private char[] Format(string text)
        {
            char[] cs = new char[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if ((c >= 0xff21) && (c <= 0xff3a)) c = (char)(c - 0xfee0);
                else if ((c >= 0xff41) && (c <= 0xff5a)) c = (char)(c - 0xff00);
                else if ((c >= 0xff10) && (c <= 0xff19)) c = (char)(c - 0xfee0);

                if (c >= 'a' && c <= 'z') c = Char.ToUpper(c);
                cs[i] = c;
            }
            return cs;
        }

        private SearchResult()
        {
            Success = false;
            Start = -1;
            End = -1;
            SearchString = null;
            Keyword = null;
        }

        public bool Success { get; private set; }
        public int Start { get; private set; }
        public int End { get; private set; }
        public string SearchString { get; private set; }
        public string Keyword { get; private set; }
        public static SearchResult Empty { get { return new SearchResult(); } }

    }
    /// <summary>
    /// 网络关键字搜索
    /// </summary>
    public class SearchKeyword
    {
        #region Objects
        class TreeNode
        {
            public TreeNode(TreeNode parent, char c, SearchKeyword key)
            {
                Char = System.Char.ToUpper(c);
                _KeywordSearch = key;
                Parent = parent;
                Results = new List<string>();
                Transitions = new List<TreeNode>();
                _transHash = new Dictionary<char, TreeNode>();
            }
            public void AddResult(string result)
            {
                if (Results.Contains(result)) return;
                Results.Add(result);
            }
            public void AddTransition(TreeNode node)
            {
                _transHash.Add(node.Char, node);
                Transitions.Add(node);
            }
            public TreeNode GetTransition(char c)
            {

                if ((c >= 0xff21) && (c <= 0xff3a)) c = (char)(c - 0xfee0);
                if ((c >= 0xff41) && (c <= 0xff5a)) c = (char)(c - 0xff00);
                if ((c >= 0xff10) && (c <= 0xff19)) c = (char)(c - 0xfee0);
                if (c >= 'a' && c <= 'z') c = System.Char.ToUpper(c);

                TreeNode node;
                if (_transHash.TryGetValue(c, out node)) { node.jumpCount = 0; return node; }

                if (c >= 'A' && c <= 'Z') return null;
                if (c >= '0' && c <= '9') return null;
                if ((c >= 0x4e00) && (c <= 0x9fa5)) return null;
                this.jumpCount++;
                if (this.jumpCount > _KeywordSearch.MaxJump) return null;
                return this;
            }
            public bool ContainsTransition(char c)
            {
                return GetTransition(c) != null;
            }

            #region Properties
            private SearchKeyword _KeywordSearch;
            private int jumpCount = 0;
            private Dictionary<char, TreeNode> _transHash;

            public bool IsFirst { get { return jumpCount == 0; } }
            public char Char { get; private set; }
            public TreeNode Parent { get; private set; }
            public TreeNode Failure { get; set; }
            public List<TreeNode> Transitions { get; private set; }
            public List<string> Results { get; private set; }
            #endregion
        }
        #endregion
        private TreeNode _root;

        public SearchKeyword(List<string> keywords) { Keywords = keywords.ToArray(); }
        public SearchKeyword(string[] keywords) { Keywords = keywords; }
        public SearchKeyword() { }

        #region Implementation
        void BuildTree()
        {
            _root = new TreeNode(null, ' ', this);
            foreach (string p in _keywords)
            {
                TreeNode nd = _root;
                var up = p.ToUpper();
                foreach (char c in up)
                {
                    TreeNode ndNew = null;
                    foreach (TreeNode trans in nd.Transitions)
                        if (trans.Char == c) { ndNew = trans; break; }

                    if (ndNew == null)
                    {
                        ndNew = new TreeNode(nd, c, this);
                        nd.AddTransition(ndNew);
                    }
                    nd = ndNew;
                }
                nd.AddResult(p);
            }

            List<TreeNode> nodes = new List<TreeNode>();
            foreach (TreeNode nd in _root.Transitions)
            {
                nd.Failure = _root;
                foreach (TreeNode trans in nd.Transitions) nodes.Add(trans);
            }
            // other nodes - using BFS �����ڵ�ʹ��BFS
            while (nodes.Count != 0)
            {
                List<TreeNode> newNodes = new List<TreeNode>();
                foreach (TreeNode nd in nodes)
                {
                    TreeNode r = nd.Parent.Failure;
                    char c = nd.Char;

                    while (r != null && !r.ContainsTransition(c)) r = r.Failure;
                    if (r == null)
                        nd.Failure = _root;
                    else
                    {
                        nd.Failure = r.GetTransition(c);
                        foreach (string result in nd.Failure.Results)
                            nd.AddResult(result);
                    }
                    // add child nodes to BFS list 
                    foreach (TreeNode child in nd.Transitions)
                        newNodes.Add(child);
                }
                nodes = newNodes;
            }
            _root.Failure = _root;
        }
        #endregion

        #region Methods & Properties
        private string[] _keywords;
        public string[] Keywords
        {
            get { return _keywords; }
            set { _keywords = value; BuildTree(); }
        }
        public int MaxJump { get; set; }

        public List<SearchResult> FindAll(string text)
        {
            List<SearchResult> ret = new List<SearchResult>();
            TreeNode ptr = _root;
            int index = 0;
            int start = 0;
            while (index < text.Length)
            {
                TreeNode trans = null;
                while (trans == null)
                {
                    trans = ptr.GetTransition(text[index]);
                    if (ptr == _root) break;
                    if (trans == null) ptr = ptr.Failure;
                }
                if (trans != null) ptr = trans;

                if (ptr.IsFirst)
                {
                    foreach (string found in ptr.Results)
                        ret.Add(new SearchResult(found, start, index, text));
                }
                index++;
                if (trans == null) start = index;
            }
            return ret;
        }

        public SearchResult FindFirst(string text)
        {
            TreeNode ptr = _root;
            int index = 0;
            int start = 0;

            while (index < text.Length)
            {
                TreeNode trans = null;
                while (trans == null)
                {
                    trans = ptr.GetTransition(text[index]);
                    if (ptr == _root) break;
                    if (trans == null) ptr = ptr.Failure;
                }
                if (trans != null) ptr = trans;

                foreach (string found in ptr.Results)
                    return new SearchResult(found, start, index, text);
                index++;
                if (trans == null) start = index;
            }
            return SearchResult.Empty;
        }
        public bool ContainsAny(string text)
        {
            TreeNode ptr = _root;
            int index = 0;

            while (index < text.Length)
            {
                TreeNode trans = null;
                while (trans == null)
                {
                    trans = ptr.GetTransition(text[index]);
                    if (ptr == _root) break;
                    if (trans == null) ptr = ptr.Failure;
                }
                if (trans != null) ptr = trans;

                if (ptr.Results.Count > 0) return true;
                index++;
            }
            return false;
        }
        #endregion
    }
    /// <summary>
    /// 完整字符搜索
    /// </summary>
    public class SearchText
    {
        #region Objects
        class TreeNode
        {
            #region Constructor & Methods

            public TreeNode(TreeNode parent, char c)
            {
                _char = c; _parent = parent;
                _results = new List<string>();

                _transitionsAr = new TreeNode[] { };
                _transHash = new Dictionary<char, TreeNode>();
            }

            public void AddResult(string result)
            {
                if (_results.Contains(result)) return;
                _results.Add(result);
            }

            public void AddTransition(TreeNode node)
            {
                _transHash.Add(node.Char, node);
                TreeNode[] ar = new TreeNode[_transHash.Values.Count];
                _transHash.Values.CopyTo(ar, 0);
                _transitionsAr = ar;
            }

            public TreeNode GetTransition(char c)
            {
                return (TreeNode)_transHash[c];
            }

            public bool ContainsTransition(char c)
            {
                return GetTransition(c) != null;
            }

            #endregion
            #region Properties

            private char _char;
            private TreeNode _parent;
            private TreeNode _failure;
            private List<string> _results;
            private TreeNode[] _transitionsAr;
            private Dictionary<char, TreeNode> _transHash;

            public char Char
            {
                get { return _char; }
            }

            public TreeNode Parent
            {
                get { return _parent; }
            }


            /// <summary>
            /// Failure function - descendant node
            /// </summary>
            public TreeNode Failure
            {
                get { return _failure; }
                set { _failure = value; }
            }


            /// <summary>
            /// Transition function - list of descendant nodes
            /// </summary>
            public TreeNode[] Transitions
            {
                get { return _transitionsAr; }
            }


            /// <summary>
            /// Returns list of patterns ending by this letter
            /// </summary>
            public List<string> Results
            {
                get { return _results; }
            }

            #endregion
        }

        #endregion
        #region Local fields

        /// <summary>
        /// Root of keyword tree
        /// </summary>
        private TreeNode _root;

        /// <summary>
        /// Keywords to search for
        /// </summary>
        private string[] _keywords;

        #endregion

        public SearchText(List<string> keywords)
        {
            Keywords = keywords.ToArray();
        }

        public SearchText(string[] keywords)
        {
            Keywords = keywords;
        }


        public SearchText() { }

        void BuildTree()
        {
            _root = new TreeNode(null, ' ');
            foreach (string p in _keywords)
            {
                // add pattern to tree
                TreeNode nd = _root;
                foreach (char c in p)
                {
                    TreeNode ndNew = null;
                    foreach (TreeNode trans in nd.Transitions)
                        if (trans.Char == c) { ndNew = trans; break; }

                    if (ndNew == null)
                    {
                        ndNew = new TreeNode(nd, c);
                        nd.AddTransition(ndNew);
                    }
                    nd = ndNew;
                }
                nd.AddResult(p);
            }

            List<TreeNode> nodes = new List<TreeNode>();
            // Find failure functions
            //ArrayList nodes = new ArrayList();
            // level 1 nodes - fail to root node
            foreach (TreeNode nd in _root.Transitions)
            {
                nd.Failure = _root;
                foreach (TreeNode trans in nd.Transitions) nodes.Add(trans);
            }
            // other nodes - using BFS
            while (nodes.Count != 0)
            {
                List<TreeNode> newNodes = new List<TreeNode>();

                //ArrayList newNodes = new ArrayList();
                foreach (TreeNode nd in nodes)
                {
                    TreeNode r = nd.Parent.Failure;
                    char c = nd.Char;

                    while (r != null && !r.ContainsTransition(c)) r = r.Failure;
                    if (r == null)
                        nd.Failure = _root;
                    else
                    {
                        nd.Failure = r.GetTransition(c);
                        foreach (string result in nd.Failure.Results)
                            nd.AddResult(result);
                    }

                    // add child nodes to BFS list 
                    foreach (TreeNode child in nd.Transitions)
                        newNodes.Add(child);
                }
                nodes = newNodes;
            }
            _root.Failure = _root;
        }


        public string[] Keywords
        {
            get { return _keywords; }
            set
            {
                _keywords = value;
                BuildTree();
            }
        }

        public List<SearchResult> FindAll(string text)
        {
            List<SearchResult> ret = new List<SearchResult>();
            TreeNode ptr = _root;
            int index = 0;

            while (index < text.Length)
            {
                TreeNode trans = null;
                while (trans == null)
                {
                    trans = ptr.GetTransition(text[index]);
                    if (ptr == _root) break;
                    if (trans == null) ptr = ptr.Failure;
                }
                if (trans != null) ptr = trans;

                foreach (string found in ptr.Results)
                    ret.Add(new SearchResult(found, index));
                index++;
            }
            return ret;
        }

        public SearchResult FindFirst(string text)
        {
            TreeNode ptr = _root;
            int index = 0;

            while (index < text.Length)
            {
                TreeNode trans = null;
                while (trans == null)
                {
                    trans = ptr.GetTransition(text[index]);
                    if (ptr == _root) break;
                    if (trans == null) ptr = ptr.Failure;
                }
                if (trans != null) ptr = trans;

                foreach (string found in ptr.Results)
                    return new SearchResult(found, index);
                index++;
            }
            return SearchResult.Empty;
        }


        public bool ContainsAny(string text)
        {
            TreeNode ptr = _root;
            int index = 0;

            while (index < text.Length)
            {
                TreeNode trans = null;
                while (trans == null)
                {
                    trans = ptr.GetTransition(text[index]);
                    if (ptr == _root) break;
                    if (trans == null) ptr = ptr.Failure;
                }
                if (trans != null) ptr = trans;

                if (ptr.Results.Count > 0) return true;
                index++;
            }
            return false;
        }

    }
}
