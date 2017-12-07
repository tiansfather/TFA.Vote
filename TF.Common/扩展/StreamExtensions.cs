using System.IO;

namespace TF.Common
{
    public static class StreamExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Converts a Stream to a String.
        /// </summary>
        /// <param name="theStream">
        /// </param>
        /// <returns>
        /// The stream to string.
        /// </returns>
        public static string AsString(this Stream theStream)
        {
            var reader = new StreamReader(theStream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// The copy stream.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="output">
        /// The output.
        /// </param>
        public static void CopyTo(this Stream input, Stream output)
        {
            var buffer = new byte[1024];
            int count = buffer.Length;

            while (count > 0)
            {
                count = input.Read(buffer, 0, count);
                if (count > 0)
                {
                    output.Write(buffer, 0, count);
                }
            }
        }

        /// <summary>
        /// Reads the stream into a byte array.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public static byte[] ToArray(this Stream stream)
        {
            var data = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(data, 0, (int)stream.Length);

            return data;
        }

        #endregion Public Methods and Operators
    }
}