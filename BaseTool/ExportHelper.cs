using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Linq;
namespace BaseTool
{
    public class ExportHelper
    {
        #region 导出图片
        public static void ExportImage(UIElement element)
        {
            var wBitmap = new WriteableBitmap(element, new MatrixTransform());
            {
                var saveDlg = new SaveFileDialog
                {
                    Filter = "JPEG图片文件 (*.jpeg)|*.jpeg",
                    DefaultExt = ".jpeg"
                };
                var showDialog = saveDlg.ShowDialog();
                if (showDialog != null && (bool)showDialog)
                {
                    using (var fs = saveDlg.OpenFile())
                    {
                        SaveToFile(wBitmap, fs);
                        MessageBox.Show("图片保存成功");
                        fs.Close();
                        fs.Dispose();
                        GC.Collect();
                    }
                }
            }
        }

        private static void SaveToFile(WriteableBitmap bitmap, Stream fs)
        {
            var width = bitmap.PixelWidth;
            var height = bitmap.PixelHeight;
            var bands = 3;
            var raster = new byte[bands][,];
            for (var i = 0; i < bands; i++)
            {
                raster[i] = new byte[width, height];
            }

            for (var row = 0; row < height; row++)
            {
                for (var column = 0; column < width; column++)
                {
                    var pixel = bitmap.Pixels[width * row + column];
                    raster[0][column, row] = (byte)(pixel >> 16);
                    raster[1][column, row] = (byte)(pixel >> 8);
                    raster[2][column, row] = (byte)pixel;
                }

            }
            var model = new FluxJpeg.Core.ColorModel { colorspace = FluxJpeg.Core.ColorSpace.RGB };
            var img = new FluxJpeg.Core.Image(model, raster);
            var stream = new MemoryStream();
            var encoder =
                new FluxJpeg.Core.Encoder.JpegEncoder(img, 100, stream);
            encoder.Encode();
            stream.Seek(0, SeekOrigin.Begin);
            var binaryData = new Byte[stream.Length];
            stream.Read(binaryData, 0, (int)stream.Length);
            fs.Write(binaryData, 0, binaryData.Length);
            stream.Close();
            stream.Dispose();
            GC.Collect();
        }
        #endregion

        #region 导出表格

            /// <summary>
        /// CSV格式化
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>格式化数据</returns>
        private static string FormatCsvField(string data)
        {
            return String.Format("\"{0}\"", data.Replace("\"", "\"\"\"").Replace("\n", "").Replace("\r", ""));
        }

        /// <summary>
        /// 导出DataGrid数据到Excel
        /// </summary>
        /// <param name="withHeaders">是否需要表头</param>
        /// <param name="grid">DataGrid</param>
        /// <returns>Excel内容字符串</returns>
        public static string ExportDataGrid(bool withHeaders, DataGrid grid)
        {
            var strBuilder = new System.Text.StringBuilder();
            var source = grid.ItemsSource as System.Collections.IList;
            if (source == null) return "";

            var headers = new List<string>();
            grid.Columns.ToList().ForEach(col =>
            {
                if (col is DataGridBoundColumn)
                {
                    //导出的结果去掉“rltGraphic”列
                    if (!col.Header.ToString().Contains("rltGraphic"))
                    {
                        headers.Add(FormatCsvField(col.Header.ToString()));
                    }

                }
            });
            strBuilder.Append(String.Join(",", headers.ToArray())).Append("\r\n");

            foreach (var data in source)
            {
                var data1 = data;
                var data2 = data;
                strBuilder.Append(String.Join(",", (from col in grid.Columns.Where(col => !col.Header.ToString().Contains("rltGraphic")).OfType<DataGridBoundColumn>() select col.Binding into binding select binding.Path.Path into colPath select data1.GetType().GetProperty(colPath) into propInfo where propInfo != null select FormatCsvField(Convert.ToString(propInfo.GetValue(data2, null)))).ToArray())).Append("\r\n");
            }
            return strBuilder.ToString();
        }
        /// <summary>
        /// 导出DataGrid数据到Excel
        /// </summary>
        /// <param name="withHeaders">是否需要表头</param>
        /// <param name="grid">DataGrid</param>
        /// <param name="dataBind">是否通过DataGridTemplateColumn设置绑定列</param>
        /// <returns>Excel内容字符串</returns>
        public static string ExportDataGrid(bool withHeaders, DataGrid grid, bool dataBind)
        {
            //string colPath;
            //System.Reflection.PropertyInfo propInfo;
            //System.Windows.Data.Binding binding;
            var strBuilder = new System.Text.StringBuilder();
            var source = grid.ItemsSource as System.Collections.IList;
            if (source == null) return "";
            var headers = new List<string>();
            grid.Columns.ToList().ForEach(col =>
            {
                if (col is DataGridTemplateColumn)
                {
                    headers.Add(col.Header != null ? FormatCsvField(col.Header.ToString()) : string.Empty);
                }
            });
            strBuilder.Append(String.Join(",", headers.ToArray())).Append("\r\n");
            foreach (var data in source)
            {
                var csvRow = new List<string>();
                foreach (var col in grid.Columns)
                {
                    if (col is DataGridTemplateColumn)
                    {
                        var cellContent = col.GetCellContent(data);
                        TextBlock block;
                        if (cellContent.GetType() == typeof(Grid))
                        {
                            block = cellContent.FindName("TempTextblock") as TextBlock;
                        }
                        else
                        {
                            block = cellContent as TextBlock;
                        }
                        if (block != null)
                        {
                            csvRow.Add(FormatCsvField(block.Text));
                        }
                    }
                }
                strBuilder.Append(String.Join(",", csvRow.ToArray())).Append("\r\n");
            }
            return strBuilder.ToString();
        }
        /// <summary>
        /// 导出DataGrid数据到Excel为CVS文件
        /// 使用utf8编码 中文是乱码  改用Unicode编码
        /// 
        /// </summary>
        /// <param name="withHeaders">是否带列头</param>
        /// <param name="grid">DataGrid</param>
        public static void ExportDataGridSaveAs(bool withHeaders, DataGrid grid)
        {
            var data = ExportDataGrid(true, grid);
            var sfd = new SaveFileDialog()
            {
                DefaultExt = "csv",
                Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*",
                FilterIndex = 1
            };
            if (sfd.ShowDialog() == true)
            {
                using (var stream = sfd.OpenFile())
                {
                    using (var writer = new StreamWriter(stream, System.Text.Encoding.Unicode))
                    {
                        data = data.Replace(",", "\t");
                        writer.Write(data);
                        writer.Close();
                        writer.Dispose();
                    }
                    stream.Close();
                    MessageBox.Show("导出成功！");
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

        #endregion 导出DataGrid数据到Excel

        #region 导出文本
        public static void ExportMsg(string[]input)
        {
            var data = input.Aggregate("", (current, s) => current + s + "\r\n");
            var sfd = new SaveFileDialog()
            {
                DefaultExt = "txt",
                DefaultFileName="数据录入情况",
                Filter = "文本文件(*.txt)|*.txt",
                FilterIndex = 1
            };
            if (sfd.ShowDialog() == true)
            {
                using (var stream = sfd.OpenFile())
                {
                    using (var writer = new StreamWriter(stream, System.Text.Encoding.Unicode))
                    {
                        writer.Write(data);
                        writer.Close();
                        writer.Dispose();
                    }
                    stream.Close();
                    MessageBox.Show("导出成功！");
                    stream.Close();
                    stream.Dispose();
                }
            }
        }
        #endregion
    }
}
