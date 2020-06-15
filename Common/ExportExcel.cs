using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PointsMall.Common
{
    public class ExportExcel
    {
        public HSSFWorkbook hssfworkbook;
        public HSSFPalette palette;
        Dictionary<string, ICellStyle> ICellStyleDictionary = new Dictionary<string, ICellStyle>();
        Dictionary<string, int> ColorDictionary = new Dictionary<string, int>();
        public ExportExcel()
        {
            this.hssfworkbook = new HSSFWorkbook();
            this.palette = this.hssfworkbook.GetCustomPalette();
            setStyle("PT", "宋体", 64, 12, true, (short)64);
        }
        /// <summary>
        /// 获取定义的单元个样式（先定义后获取）
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public ICellStyle GetICellStyle(string Name)
        {
            return ICellStyleDictionary.ContainsKey(Name) ? ICellStyleDictionary[Name] : null;
        }
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="sheetitem"></param>
        /// <returns></returns>
        public byte[] toExcel(List<Sheetitem> sheetitem)
        {
            var Sheenumber = 1;
            foreach (var Item in sheetitem)
            {

                int RowNuber = 0;
                if (string.IsNullOrEmpty(Item.Name))
                {
                    Item.Name = "shee" + Sheenumber;
                    Sheenumber++;
                }
                ISheet sheet1 = this.hssfworkbook.CreateSheet(Item.Name);
                sheet1.ForceFormulaRecalculation = true;
                sheet1.DefaultColumnWidth = 16 * 256 + 200;
                HSSFPatriarch patr = (HSSFPatriarch)sheet1.CreateDrawingPatriarch();
                //头部数据写入
                if (Item.Thread != null || Item.Thread.Count > 0)
                {
                    //IRow row0 = sheet1.CreateRow(0);
                    //ICell cell0 = row0.CreateCell(0);
                    foreach (var i in Item.Thread)
                    {
                        IRow row0 = sheet1.CreateRow(RowNuber);
                        if (i.rowHeiht > -1)
                        {
                            row0.Height = (short)(i.rowHeiht * 20);
                        }
                        ICell cell0 = row0.CreateCell(0);
                        SetColumn(i.Data, ref cell0, ref row0, ref sheet1, ref patr, ref RowNuber);
                    }
                }
                if (Item.MyTitle != null || Item.MyTitle.Count > 0)
                {
                    IRow row0 = sheet1.CreateRow(0);
                    ICell cell0 = row0.CreateCell(0);
                    this.SetColumn(Item.MyTitle, ref cell0, ref row0, ref sheet1, ref patr, ref RowNuber);
                }
                //正式数据写入
                if (Item.RowData != null || Item.RowData.Count > 0)
                {
                    foreach (var i in Item.RowData)
                    {
                        IRow row0 = sheet1.CreateRow(RowNuber);
                        if (i.rowHeiht > -1)
                        {
                            row0.Height = (short)(i.rowHeiht * 20);
                        }
                        ICell cell0 = row0.CreateCell(0);
                        SetColumn(i.Data, ref cell0, ref row0, ref sheet1, ref patr, ref RowNuber);
                    }
                }

                //尾部数据写入
                if (Item.footer != null || Item.footer.Count > 0)
                {
                    foreach (var i in Item.footer)
                    {
                        IRow row0 = sheet1.CreateRow(RowNuber);
                        if (i.rowHeiht > -1)
                        {
                            row0.Height = (short)(i.rowHeiht * 20);
                        }
                        ICell cell0 = row0.CreateCell(0);
                        SetColumn(i.Data, ref cell0, ref row0, ref sheet1, ref patr, ref RowNuber);
                    }
                }
                //合并行操作
                var AllrowData = Item.Thread.Concat(Item.RowData).Concat(Item.footer);

                foreach (var Threaditem in AllrowData)
                {
                    foreach (var CellRangeitem in Threaditem.Data.Where(e => e.CellRange != null).ToList())
                    {
                        CellRangeAddress region21 = new CellRangeAddress(CellRangeitem.CellRange.RowBegin, CellRangeitem.CellRange.RowEnd, CellRangeitem.CellRange.BeginColumn, CellRangeitem.CellRange.EndColumn);
                        sheet1.AddMergedRegion(region21);
                        if (CellRangeitem.ICellStyle != null)
                        {

                            ((HSSFSheet)sheet1).SetEnclosedBorderOfRegion(region21, CellRangeitem.ICellStyle.BorderLeft, 8);
                            ICell cell = sheet1.GetRow(CellRangeitem.CellRange.RowBegin).GetCell(CellRangeitem.CellRange.BeginColumn);
                            cell.CellStyle = CellRangeitem.ICellStyle;
                        }
                    }
                }
                //设置列宽
                foreach (var ColumnWidthitem in Item.ColumnWidth)
                {
                    sheet1.SetColumnWidth(ColumnWidthitem.Key, ColumnWidthitem.Value * 256 + 200);
                }

                //var CellRangeList = Item.Thread[0].Data.Where(e => e.CellRange != null).ToList();

            }
            return WriteToFile().GetBuffer();
        }

        /// <summary>
        /// 只读取不保存
        /// </summary>
        /// <param name="excelfile"></param>
        /// <param name="IsHaveHead"></param>
        /// <returns></returns>
        public List<Sheetitem> Import(Microsoft.AspNetCore.Http.IFormFile excelfile, bool IsHaveHead = true)
        {
            //最终结果集
            List<Sheetitem> result = new List<Sheetitem>();
            try
            {
                //工厂读取文件流
                IWorkbook readWorkbook = WorkbookFactory.Create(excelfile.OpenReadStream());
                int CountSheet = readWorkbook.NumberOfSheets;
                for (var i = 0; i < CountSheet; i++)
                {
                    //获取sheet
                    ISheet worksheet = readWorkbook.GetSheetAt(i);
                    Sheetitem item = new Sheetitem();
                    //保存sheet名字
                    item.Name = worksheet.SheetName;
                    //获取行数长度(计算是从0开始的)
                    int rowCount = worksheet.LastRowNum + 1;
                    if (rowCount == 0) { continue; }
                    int startRow = 0;
                    //需要读取的列数
                    List<int> NeedReadCol = new List<int>();
                    //如果有头部处理方式
                    if (IsHaveHead)
                    {
                        int col = -1;
                        foreach (ICell cell in worksheet.GetRow(0).Cells)
                        {
                            string CellValue = "";
                            if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                            {
                                CellValue = cell.DateCellValue.ToString();
                            }
                            else
                            {
                                CellValue = cell.ToString();
                            }
                            col++;
                            if (!string.IsNullOrEmpty(CellValue))
                            {

                                NeedReadCol.Add(col);
                                //item.Thread[0].Data.Add(new excle_option() { value = CellValue });
                                item.MyTitle.Add(new excle_option() { value = CellValue });
                            }
                        }
                        startRow++;
                    }
                    //如果不存在头部怎么全部读出来
                    else
                    {
                        for (int NeedCell = 0; NeedCell < int.Parse(worksheet.GetRow(0).LastCellNum.ToString()); NeedCell++)
                        {
                            NeedReadCol.Add(NeedCell);
                        }
                    }

                    //开始遍历所有行（如果有头部怎么去掉头部的行）
                    for (var RowNumber = startRow; RowNumber < rowCount; RowNumber++)
                    {
                        //保存每行是数据
                        RowData Row = new RowData();

                        foreach (int CellNumber in NeedReadCol)
                        {
                            string CellValue = "";
                            ICell Cell = worksheet.GetRow(RowNumber).GetCell(CellNumber);
                            if (Cell == null)
                            {
                                CellValue = "";
                            }
                            else if (Cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(Cell))
                            {
                                CellValue = Cell.DateCellValue.ToString();
                            }
                            else
                            {
                                CellValue = Cell.ToString();
                            }
                            //每个但单元格的数据
                            excle_option DataCell = new excle_option();
                            DataCell.value = CellValue;
                            //将单元的数据加到行中
                            Row.Data.Add(DataCell);
                        }
                        if (Row.Data.FindAll(e => !string.IsNullOrEmpty(e.value)).Count > 0)
                        {
                            //遍历完行后，添加到sheet中
                            item.RowData.Add(Row);
                        }
                    }
                    //将sheet添加到结果集中
                    result.Add(item);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }
        /// <summary>
        /// 设置每列数据
        /// </summary>
        /// <param name="option"></param>
        /// <param name="cell0"></param>
        /// <param name="row0"></param>
        /// <param name="sheet1"></param>
        /// <param name="patr"></param>
        /// <param name="RowNuber"></param>
        private void SetColumn(List<excle_option> option, ref ICell cell0, ref IRow row0, ref ISheet sheet1, ref HSSFPatriarch patr, ref int RowNuber)
        {
            int ColumnNum = 0;
            if (option != null)
            {

                for (int i = 0; i < option.Count; i++)
                {
                    if (option[i].index != null)
                    {
                        ColumnNum = int.Parse(option[i].index.ToString());
                    }
                    else
                    {
                        ColumnNum = i;
                    }
                    cell0 = row0.CreateCell(ColumnNum);
                    //设置批注和
                    if (option[i].Description == null || option[i].Description == "")
                    {
                        switch (option[i].TypeOfValue)
                        {
                            case 0: cell0.SetCellValue(option[i].value); break;
                            case 1: bool result = false; if (bool.TryParse(option[i].value, out result)) { cell0.SetCellValue(result); } else { cell0.SetCellValue(option[i].value); } break;
                            case 2: DateTime DateResult = new DateTime(); if (DateTime.TryParse(option[i].value, out DateResult)) { cell0.SetCellValue(DateResult); } else { cell0.SetCellValue(option[i].value); } break;
                            case 3: double DoubleResult = new double(); if (double.TryParse(option[i].value, out DoubleResult)) { cell0.SetCellValue(DoubleResult); } else { cell0.SetCellValue(option[i].value); } break;
                            case 4: cell0.SetCellFormula(option[i].value); break;
                            default: cell0.SetCellValue(option[i].value); break;
                        }
                    }
                    else
                    {
                        cell0.SetCellValue(new HSSFRichTextString(option[i].value));
                        cell0.CellComment = (addPiZhu(patr, option[i].Description, ""));
                    }
                    //赋值单元格样式
                    if (option[i].ICellStyle != null)
                    {
                        cell0.CellStyle = option[i].ICellStyle;
                    }
                    if (option[i].option != null)
                    {
                        CellRangeAddressList rangeList = new CellRangeAddressList();
                        rangeList.AddCellRangeAddress(new CellRangeAddress(1, 100, ColumnNum, ColumnNum));
                        DVConstraint dvconstraint = DVConstraint.CreateExplicitListConstraint(option[i].option);
                        HSSFDataValidation dataValidation = new HSSFDataValidation(rangeList, dvconstraint);
                        //add the data validation to sheet1
                        ((HSSFSheet)sheet1).AddValidationData(dataValidation);
                    }
                    //ColumnNum++;
                }
            }
            RowNuber++;
        }

        /// <summary>
        /// 获取字体样式
        /// </summary>
        /// <param name="fontfamily"></param>
        /// <param name = "fontcolor" > 字体颜色 </param >
        /// <param name="fontsize">字体大小</param>
        /// <param name="BoldWeight"></param>
        /// <returns></returns>
        private IFont GetFontStyle(string fontfamily, short fontcolor, int fontsize, short BoldWeight = 400)
        {
            IFont font1 = this.hssfworkbook.CreateFont();
            font1.FontName = fontfamily;
            font1.Color = fontcolor;
            font1.FontHeightInPoints = (short)fontsize;
            font1.Boldweight = BoldWeight;
            return font1;
        }
        /// <summary>
        /// 设置单元格式
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="fontfamily"></param>
        /// <param name="fontcolor">颜色请查询NPOI对应颜色</param>
        /// <param name="fontsize"></param>
        /// <param name="IsBorder"></param>
        /// <param name="FillBackgroundColor"></param>
        /// <param name="_HorizontalAlignment">1：居中 2右对齐 3左对齐</param>
        /// <param name="_VerticalAlignment">1：居中 2居上 3居下</param>
        /// <param name="BoldWeight"></param>
        public void setStyle(string Name, string fontfamily, short fontcolor, int fontsize, bool IsBorder, short FillBackgroundColor, int _HorizontalAlignment = 0, int _VerticalAlignment = 0, short BoldWeight = 400)
        {
            ICellStyle item = this.hssfworkbook.CreateCellStyle();
            item.SetFont(GetFontStyle(fontfamily, fontcolor, fontsize, BoldWeight));
            if (IsBorder)
            {
                item.BorderBottom = BorderStyle.Thin;
                item.BottomBorderColor = 8;
                item.BorderLeft = BorderStyle.Thin;
                item.LeftBorderColor = 8;
                item.BorderRight = BorderStyle.Thin;
                item.RightBorderColor = 8;
                item.BorderTop = BorderStyle.Thin;
                item.TopBorderColor = 8;
            }
            //水平样式
            if (_HorizontalAlignment != 0)
            {
                switch (_HorizontalAlignment)
                {
                    //水平居中
                    case 1: item.Alignment = HorizontalAlignment.Center; break;
                    //右对青
                    case 2: item.Alignment = HorizontalAlignment.Right; break;
                    //左对齐
                    case 3: item.Alignment = HorizontalAlignment.Left; break;
                    default: break;
                }
            }
            //垂直样式
            if (_VerticalAlignment != 0)
            {
                switch (_HorizontalAlignment)
                {
                    //水平居中
                    case 1: item.VerticalAlignment = VerticalAlignment.Center; break;
                    //居上
                    case 2: item.VerticalAlignment = VerticalAlignment.Top; break;
                    //居下
                    case 3: item.VerticalAlignment = VerticalAlignment.Bottom; break;

                    default: break;
                }
            }

            //item.FillForegroundColor = FillBackgroundColor;
            item.FillForegroundColor = FillBackgroundColor;
            item.FillPattern = FillPattern.SolidForeground;
            //item.FillBackgroundColor = HSSFColor.SkyBlue.Index;
            this.ICellStyleDictionary.Add(Name, item);
            //return item;
        }
        /// <summary>
        /// 设置单元格式
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="fontfamily"></param>
        /// <param name="fontcolor">设置后的颜色名称</param>
        /// <param name="fontsize"></param>
        /// <param name="IsBorder"></param>
        /// <param name="FillBackgroundColor">设置后的颜色名称</param>
        /// <param name="BoldWeight"></param>
        public ICellStyle setStyle(string Name, string fontfamily, string fontcolor, int fontsize, bool IsBorder, string FillBackgroundColor, short BoldWeight = 400)
        {
            ICellStyle item = this.hssfworkbook.CreateCellStyle();
            short Fontcolor = 64;
            short fillBackgroundColor = 64;
            if (this.ColorDictionary.ContainsKey(fontcolor))
            {
                Fontcolor = (short)this.ColorDictionary[fontcolor];
            }
            if (this.ColorDictionary.ContainsKey(FillBackgroundColor))
            {
                fillBackgroundColor = (short)this.ColorDictionary[FillBackgroundColor];
            }
            item.SetFont(GetFontStyle(fontfamily, Fontcolor, fontsize, BoldWeight));
            if (IsBorder)
            {
                item.BorderBottom = BorderStyle.Thin;
                item.BottomBorderColor = fillBackgroundColor;
                item.BorderLeft = BorderStyle.Thin;
                item.LeftBorderColor = fillBackgroundColor;
                item.BorderRight = BorderStyle.Thin;
                item.RightBorderColor = fillBackgroundColor;
                item.BorderTop = BorderStyle.Thin;
                item.TopBorderColor = fillBackgroundColor;
            }
            //item.FillForegroundColor = FillBackgroundColor;
            item.FillForegroundColor = fillBackgroundColor;
            item.FillPattern = FillPattern.SolidForeground;
            //item.FillBackgroundColor = HSSFColor.SkyBlue.Index;
            this.ICellStyleDictionary.Add(Name, item);
            return item;
        }
        //添加批注
        public IComment addPiZhu(HSSFPatriarch patr, string commond, string Author)
        {
            IComment comment1 = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, 4, 3, 6, 5));
            comment1.String = (new HSSFRichTextString(commond));
            comment1.Author = (Author);
            return comment1;
        }
        private MemoryStream WriteToFile()
        {
            MemoryStream filestream = new MemoryStream(); //内存文件流(应该可以写成普通的文件流)
            this.hssfworkbook.Write(filestream); //把文件读到内存流里面
            return filestream;
        }
        /// <summary>
        /// 自定义颜色
        /// </summary>
        /// <param name="Index">范围8-64（会覆盖原编号颜色）</param>
        /// <param name="Red">R</param>
        /// <param name="Greed">G</param>
        /// <param name="Blue">B</param>
        /// <param name="Name">颜色名称</param>
        public void SetColortoNpoi(int Index, int Red, int Greed, int Blue, string Name)
        {
            this.palette.SetColorAtIndex((short)Index, (byte)Red, (byte)Greed, (byte)Blue);
            this.ColorDictionary.Add(Name, Index);
        }
        /// <summary>
        /// 将Excel列转成数字
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>

        public int ToIndex(string columnName)
        {
            if (!Regex.IsMatch(columnName.ToUpper(), @"[A-Z]+")) { throw new Exception("invalid parameter"); }

            int index = 0;
            char[] chars = columnName.ToUpper().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                index += ((int)chars[i] - (int)'A' + 1) * (int)Math.Pow(26, chars.Length - i - 1);
            }
            return index - 1;
        }
        /// <summary>
        /// 数字转excel列名
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string ToName(int index)
        {
            if (index < 0) { throw new Exception("invalid parameter"); }

            List<string> chars = new List<string>();
            do
            {
                if (chars.Count > 0) index--;
                chars.Insert(0, ((char)(index % 26 + (int)'A')).ToString());
                index = (int)((index - index % 26) / 26);
            } while (index > 0);

            return String.Join(string.Empty, chars.ToArray());
        }
    }

    #region Excel数据导出使用的实体类
    /// <summary>
    /// 详细的单元格数据
    /// </summary>
    public class excle_option
    {
        public excle_option()
        {
            TypeOfValue = 0;
            this.CellRange = null;
        }
        /// <summary>
        /// 单元格内容格式：0：string 1:bool 2:DateTime 3 double 4 公式 默认0；
        /// </summary>
        public int TypeOfValue { get; set; }
        /// <summary>
        /// 第几列
        /// </summary>
        public int? index { get; set; }
        /// <summary>
        /// 项目值
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 项目选择如无则传NULL
        /// </summary>
        public string[] option { get; set; }
        /// <summary>
        /// 项目描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 单元格样式
        /// </summary>
        public ICellStyle ICellStyle { get; set; }
        /// <summary>
        /// 设置合并行列
        /// </summary>
        public CellRange CellRange { get; set; }

    }
    /// <summary>
    /// excel钟的一个sheet对象
    /// </summary>
    public class Sheetitem
    {
        /// <summary>
        /// Sheet名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Sheet头部
        /// </summary>
        public List<RowData> Thread { get; set; }
        /// <summary>
        /// Sheet数据
        /// </summary>
        public List<RowData> RowData { get; set; }

        public List<excle_option> MyTitle { get; set; }
        /// <summary>
        /// sheet尾部
        /// </summary>
        public List<RowData> footer { get; set; }
        /// <summary>
        /// 设置列宽度（可不填）key 行号 ；value ：宽度
        /// </summary>
        public Dictionary<int, int> ColumnWidth { get; set; }
        public Sheetitem()
        {
            this.Thread = new List<RowData>();
            this.RowData = new List<RowData>();
            this.footer = new List<RowData>();
            this.MyTitle = new List<excle_option>();
            this.ColumnWidth = new Dictionary<int, int>();
        }
    }
    /// <summary>
    /// 每行的数据
    /// </summary>
    public class RowData
    {
        /// <summary>
        /// 行高
        /// </summary>
        public int rowHeiht { get; set; }
        public List<excle_option> Data { get; set; }
        public RowData()
        {
            this.rowHeiht = -1;
            this.Data = new List<excle_option>();
        }
    }
    public class SO_excle_option : excle_option
    {
        public SSTemplateOptionType Type { get; set; }
    }
    public class IDValue
    {
        //标识
        public string ID { get; set; }
        //值
        public string Value { get; set; }
        //状态
        public string State { get; set; }
    }
    public enum SSTemplateOptionType
    {
        /// <summary>
        /// 社保
        /// </summary>
        SO = 1,
        /// <summary>
        /// 公积金
        /// </summary>
        GJJ = 2,
        /// <summary>
        /// 其他
        /// </summary>
        Other = 3,
        /// <summary>
        /// 常规字段
        /// </summary>
        PT = 4,
        /// <summary>
        /// 备注
        /// </summary>
        mark = 5
    }
    /// <summary>
    /// 合并单元格
    /// </summary>
    public class CellRange
    {
        /// <summary>
        /// 起始行
        /// </summary>
        public int RowBegin { get; set; }
        /// <summary>
        /// 结束行
        /// </summary>
        public int RowEnd { get; set; }
        /// <summary>
        /// 起始列
        /// </summary>
        public int BeginColumn { get; set; }
        /// <summary>
        /// 结束列
        /// </summary>
        public int EndColumn { get; set; }
    }
    #endregion
}