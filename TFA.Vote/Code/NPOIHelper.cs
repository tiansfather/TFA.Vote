﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Data;
using System.Drawing;

namespace TFA.Vote
{
    public class ExcelHelper : IDisposable
    {
        private string fileName = null; //文件名
        private IWorkbook workbook = null;
        private FileStream fs = null;
        private bool disposed;

        public ExcelHelper(string fileName)
        {
            this.fileName = fileName;
            disposed = false;
        }

        public IWorkbook WorkBook
        {
            get
            {
                return workbook;
            }
        }
        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public int DataTableToExcel(DataTable data, string sheetName, bool isColumnWritten)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;

            fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook();
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);
                }
                else
                {
                    return -1;
                }

                if (isColumnWritten == true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        var headcell = row.CreateCell(j);
                        headcell.SetCellValue(data.Columns[j].ColumnName);
                        var headcellstyle = workbook.CreateCellStyle();
                        headcellstyle.Alignment = HorizontalAlignment.Center;
                        var c = data.Columns[j].Prefix.Replace("c","");
                        if (!c.IsNullOrEmpty())
                        {
                            headcellstyle.FillForegroundColor = Convert.ToInt16(c);
                            headcellstyle.FillPattern = FillPattern.SolidForeground;
                        }
                        headcell.CellStyle = headcellstyle;
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        var cell = row.CreateCell(j);
                        cell.SetCellValue(data.Rows[i][j].ToString());
                        var cellstyle = workbook.CreateCellStyle();
                        cellstyle.Alignment = HorizontalAlignment.Center;
                        var c = data.Columns[j].Prefix.Replace("c", "");
                        if (!c.IsNullOrEmpty())
                        {
                            cellstyle.FillForegroundColor = Convert.ToInt16(c);
                            cellstyle.FillPattern = FillPattern.SolidForeground;
                        }
                        cell.CellStyle = cellstyle;
                    }
                    ++count;
                }
                //列宽自适应，只对英文和数字有效
                for ( i = 0; i <= data.Rows.Count; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
                //获取当前列的宽度，然后对比本列的长度，取最大值
                for (int columnNum = 0; columnNum <= data.Columns.Count; columnNum++)
                {
                    int columnWidth = sheet.GetColumnWidth(columnNum) / 256;
                    for (int rowNum = 1; rowNum <= sheet.LastRowNum; rowNum++)
                    {
                        IRow currentRow;
                        //当前行未被使用过
                        if (sheet.GetRow(rowNum) == null)
                        {
                            currentRow = sheet.CreateRow(rowNum);
                        }
                        else
                        {
                            currentRow = sheet.GetRow(rowNum);
                        }

                        if (currentRow.GetCell(columnNum) != null)
                        {
                            ICell currentCell = currentRow.GetCell(columnNum);
                            int length = Encoding.Default.GetBytes(currentCell.ToString()).Length;
                            if (columnWidth < length)
                            {
                                columnWidth = length;
                            }
                        }
                    }
                    sheet.SetColumnWidth(columnNum, Math.Max(10,columnWidth) * 256);
                }

                workbook.Write(fs); //写入到excel
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public DataTable ExcelToDataTable(string sheetName, bool isFirstRowColumn)
        {
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);

                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue.Trim());
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {

                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                            {
                                ICell cell = row.GetCell(j);
                                if (cell.CellType == CellType.Numeric)
                                {
                                    //NPOI中数字和日期都是NUMERIC类型的，这里对其进行判断是否是日期类型
                                    if (HSSFDateUtil.IsCellDateFormatted(cell))//日期类型
                                    {
                                        dataRow[j] = cell.DateCellValue.ToString("yyyy-MM-dd");
                                    }
                                    else
                                    {
                                        dataRow[j] = cell.NumericCellValue;
                                    }
                                }
                                else
                                {
                                    dataRow[j] = row.GetCell(j).ToString().Trim();
                                }
                            }
                        }
                        data.Rows.Add(dataRow);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (fs != null)
                        fs.Close();
                }

                fs = null;
                disposed = true;
            }
        }
    }

    public class PicturesInfo
    {
        public int MinRow { get; set; }
        public int MaxRow { get; set; }
        public int MinCol { get; set; }
        public int MaxCol { get; set; }
        public Byte[] PictureData { get; private set; }

        public PicturesInfo(int minRow, int maxRow, int minCol, int maxCol, Byte[] pictureData)
        {
            this.MinRow = minRow;
            this.MaxRow = maxRow;
            this.MinCol = minCol;
            this.MaxCol = maxCol;
            this.PictureData = pictureData;
        }
    }

    public static class NpoiExtend
    {
        public static List<PicturesInfo> GetAllPictureInfos(this ISheet sheet)
        {
            return sheet.GetAllPictureInfos(null, null, null, null);
        }

        public static List<PicturesInfo> GetAllPictureInfos(this ISheet sheet, int? minRow, int? maxRow, int? minCol, int? maxCol, bool onlyInternal = true)
        {
            if (sheet is HSSFSheet)
            {
                return GetAllPictureInfos((HSSFSheet)sheet, minRow, maxRow, minCol, maxCol, onlyInternal);
            }
            else if (sheet is XSSFSheet)
            {
                return GetAllPictureInfos((XSSFSheet)sheet, minRow, maxRow, minCol, maxCol, onlyInternal);
            }
            else
            {
                throw new Exception("未处理类型，没有为该类型添加：GetAllPicturesInfos()扩展方法！");
            }
        }

        private static List<PicturesInfo> GetAllPictureInfos(HSSFSheet sheet, int? minRow, int? maxRow, int? minCol, int? maxCol, bool onlyInternal)
        {
            List<PicturesInfo> picturesInfoList = new List<PicturesInfo>();

            var shapeContainer = sheet.DrawingPatriarch as HSSFShapeContainer;
            if (null != shapeContainer)
            {
                var shapeList = shapeContainer.Children;
                foreach (var shape in shapeList)
                {
                    if (shape is HSSFPicture)
                    {
                        var picture = (HSSFPicture)shape;
                        // var anchor = (HSSFClientAnchor)shape;
                        var anchor = (HSSFClientAnchor)picture.Anchor;
                        if (IsInternalOrIntersect(minRow, maxRow, minCol, maxCol, anchor.Row1, anchor.Row2, anchor.Col1, anchor.Col2, onlyInternal))
                        {
                            picturesInfoList.Add(new PicturesInfo(anchor.Row1, anchor.Row2, anchor.Col1, anchor.Col2, picture.PictureData.Data));
                        }
                    }
                }
            }
            picturesInfoList.Sort(delegate (PicturesInfo x, PicturesInfo y)
            {
                return x.MinRow.CompareTo(y.MinRow);
            });
            return picturesInfoList;
        }

        private static List<PicturesInfo> GetAllPictureInfos(XSSFSheet sheet, int? minRow, int? maxRow, int? minCol, int? maxCol, bool onlyInternal)
        {
            List<PicturesInfo> picturesInfoList = new List<PicturesInfo>();

            var documentPartList = sheet.GetRelations();
            foreach (var documentPart in documentPartList)
            {
                if (documentPart is XSSFDrawing)
                {
                    var drawing = (XSSFDrawing)documentPart;
                    var shapeList = drawing.GetShapes();
                    foreach (var shape in shapeList)
                    {
                        if (shape is XSSFPicture)
                        {
                            var picture = (XSSFPicture)shape;
                            var anchor = picture.GetPreferredSize();

                            if (IsInternalOrIntersect(minRow, maxRow, minCol, maxCol, anchor.Row1, anchor.Row2, anchor.Col1, anchor.Col2, onlyInternal))
                            {
                                picturesInfoList.Add(new PicturesInfo(anchor.Row1, anchor.Row2, anchor.Col1, anchor.Col2, picture.PictureData.Data));
                            }
                        }
                    }
                }
            }

            return picturesInfoList;
        }

        private static bool IsInternalOrIntersect(int? rangeMinRow, int? rangeMaxRow, int? rangeMinCol, int? rangeMaxCol,
            int pictureMinRow, int pictureMaxRow, int pictureMinCol, int pictureMaxCol, bool onlyInternal)
        {
            int _rangeMinRow = rangeMinRow ?? pictureMinRow;
            int _rangeMaxRow = rangeMaxRow ?? pictureMaxRow;
            int _rangeMinCol = rangeMinCol ?? pictureMinCol;
            int _rangeMaxCol = rangeMaxCol ?? pictureMaxCol;

            if (onlyInternal)
            {
                return (_rangeMinRow <= pictureMinRow && _rangeMaxRow >= pictureMaxRow &&
                        _rangeMinCol <= pictureMinCol && _rangeMaxCol >= pictureMaxCol);
            }
            else
            {
                return ((Math.Abs(_rangeMaxRow - _rangeMinRow) + Math.Abs(pictureMaxRow - pictureMinRow) >= Math.Abs(_rangeMaxRow + _rangeMinRow - pictureMaxRow - pictureMinRow)) &&
                (Math.Abs(_rangeMaxCol - _rangeMinCol) + Math.Abs(pictureMaxCol - pictureMinCol) >= Math.Abs(_rangeMaxCol + _rangeMinCol - pictureMaxCol - pictureMinCol)));
            }
        }

    }
}