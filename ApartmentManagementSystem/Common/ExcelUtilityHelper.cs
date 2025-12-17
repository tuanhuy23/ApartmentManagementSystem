using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApartmentManagementSystem.Common
{
    public static class ExcelUtilityHelper
    {
        public class ExcelHeader
        {
            public string ColumnName { get; set; }
            public string ColumnValue { get; set; }
        }

        public class ExcelData
        {
            public List<ExcelHeader> header { get; set; }
            public List<Dictionary<string, object>> body { get; set; }
        }

        public static byte[] ExportToExcel(string fileName, string sheetName, string strData)
        {
            var jsonData = JsonConvert.DeserializeObject<JObject>(strData);

            // Get the header and body information from the JObject
            var header = jsonData["header"].ToObject<JArray>();
            var body = jsonData["body"].ToObject<JArray>();

            // Create arrays for headers and values
            var headers = header.Select(item => item.First.ToString()).ToArray();
            var values = new string[body.Count, headers.Length];

            // Populate the values array from the body data
            for (int i = 0; i < body.Count; i++)
            {
                var rowData = body[i].ToObject<JObject>();
                for (int j = 0; j < headers.Length; j++)
                {
                    var columnName = headers[j].Split(':')[0].Trim('"');
                    var value = rowData.GetValue(columnName)?.ToString();
                    values[i, j] = value ?? "";
                }
            }

            headers = header.Select(item => item.First.First.ToString()).ToArray();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(sheetName);

                for (int col = 0; col < headers.Length; col++)
                {
                    worksheet.Cell(1, col + 1).Value = headers[col];
                }

                for (int row = 0; row < values.GetLength(0); row++)
                {
                    for (int col = 0; col < values.GetLength(1); col++)
                    {
                        worksheet.Cell(row + 2, col + 1).Value = values[row, col];
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
        public static ExcelData ImportFromExcel(IFormFile file)
        {
            try
            {
                var jsonData = new ExcelData
                {
                    header = new List<ExcelHeader>(),
                    body = new List<Dictionary<string, object>>()
                };

                IWorkbook workbook;
                using (var stream = file.OpenReadStream())
                {
                    if (file.FileName.EndsWith(".xls"))
                    {
                        workbook = new HSSFWorkbook(stream);
                    }
                    else if (file.FileName.EndsWith(".xlsx"))
                    {
                        workbook = new XSSFWorkbook(stream);
                    }
                    else
                    {
                        throw new Exception("Unsupported file format");
                    }

                    ISheet sheet = workbook.GetSheetAt(0);
                    IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();
                    var headerRow = sheet.GetRow(sheet.FirstRowNum);
                    var colCount = headerRow.LastCellNum;
                    var dicHeaders = new Dictionary<string, string>();
                    
                    for (int col = 0; col < colCount; col++)
                    {
                        var cell = headerRow.GetCell(col, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                        var cellValue = evaluator.EvaluateInCell(cell).ToString();
                        jsonData.header.Add(new ExcelHeader
                        {
                            ColumnName = "column" + (col + 1),
                            ColumnValue = cellValue
                        });
                        dicHeaders.Add("column" + (col + 1), cellValue);
                    }

                    
                    for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                    {
                        var row = sheet.GetRow(rowIndex);
                        if (row == null) continue;
                        if (IsRowEmpty(row)) continue;
                        Dictionary<string, object> bodyRow = new Dictionary<string, object>();
                        for (int col = 0; col < colCount; col++)
                        {
                            var cell = row.GetCell(col, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                            evaluator.EvaluateInCell(cell);
                            string resultString = "";

                            if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                            {
                                var dateValue = cell.DateCellValue;
                                resultString = dateValue?.ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                DataFormatter dataFormatter = new DataFormatter();
                                resultString = dataFormatter.FormatCellValue(cell);
                            }
                            var headerValue = dicHeaders["column" + (col + 1)];
                            bodyRow.Add(headerValue, resultString);
                        }

                        jsonData.body.Add(bodyRow);
                    }
                }

                return jsonData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        static bool IsRowEmpty(IRow row)
        {
            if (row == null) return true;
            for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
            {
                ICell cell = row.GetCell(j);
                if (cell != null && cell.CellType != CellType.Blank)
                {
                    if (!string.IsNullOrWhiteSpace(cell.ToString()))
                    {
                        return false; 
                    }
                }
            }
            return true;
        }
    }
}