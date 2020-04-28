using iTextSharp.text;
using iTextSharp.text.pdf;
using LNF.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LNF.Printing
{
    public static class StorePrintManager
    {
        public static byte[] GetStoreOrderPdf(IStoreOrder order)
        {
            MemoryStream ms1 = new MemoryStream();
            MemoryStream ms2 = new MemoryStream();
            Document doc = new Document();
            PdfWriter.GetInstance(doc, ms1);
            doc.Open();

            doc.Add(GetTitle());
            doc.Add(GetHeader(order));
            GetDetail(doc, order);
            doc.Add(GetFooter(order));

            doc.Close();

            //add page number footer
            PdfReader reader = new PdfReader(ms1.ToArray());
            PdfStamper stamper = new PdfStamper(reader, ms2);
            int n = reader.NumberOfPages;
            for (int i = 1; i <= n; i++)
                GetPageFooter(order, i, n, doc.PageSize.Width - 20).WriteSelectedRows(0, -1, 10, 30, stamper.GetOverContent(i));

            stamper.Close();
            reader.Close();

            return ms2.GetBuffer();
        }

        private static Paragraph GetError(int soid)
        {
            Paragraph p = new Paragraph(string.Format("Error: Could not find Order # {0}", soid), new Font(Font.FontFamily.HELVETICA, 18, Font.NORMAL, BaseColor.RED))
            {
                Alignment = Element.ALIGN_CENTER
            };

            return p;
        }

        private static Paragraph GetTitle()
        {
            Paragraph p = new Paragraph("LNF Online Store Order", new Font(Font.FontFamily.HELVETICA, 18))
            {
                Alignment = Element.ALIGN_CENTER
            };

            return p;
        }

        private static PdfPTable GetHeader(IStoreOrder order)
        {
            PdfPTable container = new PdfPTable(1)
            {
                SpacingBefore = 20
            };

            container.SetTotalWidth(new float[] { 420 });
            container.LockedWidth = true;

            PdfPTable info = new PdfPTable(2);
            info.AddCell(GetOrderInfo(order));
            info.AddCell(GetCustomerInfo(order));

            PdfPCell containerCell = new PdfPCell(info)
            {
                Border = AllBorders(),
                Padding = 1
            };

            container.AddCell(containerCell);

            return container;
        }

        private static PdfPCell GetOrderInfo(IStoreOrder order)
        {
            PdfPTable table = new PdfPTable(1);

            PdfPTable innerTable = new PdfPTable(new float[] { 0.3F, 0.7F });
            innerTable.AddCell(HeaderCell("ORDER INFO"));

            //first row
            innerTable.AddCell(new PdfPCell(BoldPhrase("Order #:")) { Border = Rectangle.NO_BORDER, FixedHeight = 16 });
            innerTable.AddCell(new PdfPCell(NormalPhrase(order.SOID.ToString())) { Border = Rectangle.NO_BORDER, FixedHeight = 16 });

            //second row
            innerTable.AddCell(new PdfPCell(BoldPhrase("Order Date:")) { Border = Rectangle.NO_BORDER, FixedHeight = 16 });
            innerTable.AddCell(new PdfPCell(NormalPhrase(order.CreationDate.ToString("M/d/yyyy"))) { Border = Rectangle.NO_BORDER, FixedHeight = 16 });

            //third row
            innerTable.AddCell(new PdfPCell(BoldPhrase("Order Type:")) { Border = Rectangle.NO_BORDER, FixedHeight = 16 });
            innerTable.AddCell(new PdfPCell(NormalPhrase(GetOrderType(order))) { Border = Rectangle.NO_BORDER, FixedHeight = 16 });

            //fourth row
            innerTable.AddCell(new PdfPCell(BoldPhrase("Pickup:")) { Border = Rectangle.NO_BORDER, FixedHeight = 16 });
            innerTable.AddCell(new PdfPCell(NormalPhrase(StoreOrders.GetPickupLocation(order))) { Border = Rectangle.NO_BORDER, FixedHeight = 16 });

            PdfPCell innerCell = new PdfPCell(innerTable)
            {
                Border = AllBorders(),
                Padding = 1
            };

            table.AddCell(innerCell);

            PdfPCell outerCell = new PdfPCell(table)
            {
                Border = Rectangle.NO_BORDER,
                Padding = 1
            };

            return outerCell;
        }

        private static PdfPCell GetCustomerInfo(IStoreOrder order)
        {
            PdfPTable outerTable = new PdfPTable(1);

            PdfPTable innerTable = new PdfPTable(new float[] { 0.3F, 0.7F });

            var c = ServiceProvider.Current.Data.Client.GetClient(order.ClientID);

            //header row
            innerTable.AddCell(HeaderCell("CUSTOMER INFO"));

            //first row
            innerTable.AddCell(new PdfPCell(BoldPhrase("Name:")) { Border = Rectangle.NO_BORDER, FixedHeight = 16 });
            innerTable.AddCell(new PdfPCell(NormalPhrase(c.DisplayName)) { Border = Rectangle.NO_BORDER, FixedHeight = 16 });

            //second row
            innerTable.AddCell(new PdfPCell(BoldPhrase("Contact:")) { Border = Rectangle.NO_BORDER, FixedHeight = 16 });
            innerTable.AddCell(new PdfPCell(NormalPhrase(c.Email)) { Border = Rectangle.NO_BORDER, FixedHeight = 16 });

            //third row
            innerTable.AddCell(new PdfPCell(BoldPhrase("")) { Border = Rectangle.NO_BORDER, FixedHeight = 16 });
            innerTable.AddCell(new PdfPCell(NormalPhrase(c.Phone)) { Border = Rectangle.NO_BORDER, FixedHeight = 16 });

            //fourth row
            innerTable.AddCell(new PdfPCell(BoldPhrase("Account:")) { Border = Rectangle.NO_BORDER, FixedHeight = 16 });
            innerTable.AddCell(new PdfPCell(NormalPhrase(order.AccountName)) { Border = Rectangle.NO_BORDER, FixedHeight = 16 });

            PdfPCell innerCell = new PdfPCell(innerTable)
            {
                Border = AllBorders(),
                Padding = 1
            };

            outerTable.AddCell(innerCell);

            PdfPCell outerCell = new PdfPCell(outerTable)
            {
                Border = Rectangle.NO_BORDER,
                Padding = 1
            };

            return outerCell;
        }

        private static PdfPTable GetPageFooter(IStoreOrder order, int pageNumber, int totalPages, float width)
        {
            PdfPTable tbl = new PdfPTable(3)
            {
                TotalWidth = width
            };

            Phrase p;
            PdfPCell cell;

            p = new Phrase(string.Format("Order #{0}", order.SOID), new Font(Font.FontFamily.HELVETICA, 8, Font.ITALIC, BaseColor.LIGHT_GRAY));
            cell = new PdfPCell(p) { Border = Rectangle.TOP_BORDER, BorderColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_LEFT };
            tbl.AddCell(cell);

            p = new Phrase(string.Format("Printed {0:M/d/yyyy h:mm tt}", DateTime.Now), new Font(Font.FontFamily.HELVETICA, 8, Font.ITALIC, BaseColor.LIGHT_GRAY));
            cell = new PdfPCell(p) { Border = Rectangle.TOP_BORDER, BorderColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_CENTER };
            tbl.AddCell(cell);

            p = new Phrase(string.Format("Page {0} of {1}", pageNumber, totalPages), new Font(Font.FontFamily.HELVETICA, 8, Font.ITALIC, BaseColor.LIGHT_GRAY));
            cell = new PdfPCell(p) { Border = Rectangle.TOP_BORDER, BorderColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_RIGHT };
            tbl.AddCell(cell);

            return tbl;
        }

        private static void GetDetail(Document doc, IStoreOrder order)
        {
            IList<IStoreOrderDetail> details = StoreOrderDetails.GetDetails(order.SOID).ToList();

            if (details != null && details.Count > 0)
            {
                int index = 0;
                int pageNumber = 0;
                float firstPageHeight = 550;
                float secondPageHeight = 690;

                BaseColor itemColor = new BaseColor(238, 238, 238);
                BaseColor altItemColor = BaseColor.WHITE;
                BaseColor bc = itemColor;

                PdfPTable innerTable = GetDetailPageTable();

                void appendOuterTable(PdfPTable tbl, int pageNum, bool isLast)
                {
                    PdfPTable outerTable = new PdfPTable(1)
                    {
                        SpacingBefore = (pageNum == 0) ? 10 : 0
                    };

                    PdfPCell outerCell = new PdfPCell(tbl)
                    {
                        Border = AllBorders(),
                        Padding = 2
                    };

                    if (isLast)
                    {
                        var tableHeight = innerTable.TotalHeight;
                        var pageHeight = (pageNum == 0) ? firstPageHeight : secondPageHeight;
                        if (tableHeight < pageHeight)
                            outerCell.FixedHeight = pageHeight;
                    }

                    outerTable.AddCell(outerCell);

                    doc.Add(outerTable);

                    //if (!isLast)
                    //    doc.Add(GetFooter(order, true));
                }

                foreach (IStoreOrderDetail item in details)
                {
                    float h = (pageNumber == 0) ? firstPageHeight : secondPageHeight;

                    if (innerTable.TotalHeight >= h)
                    {
                        appendOuterTable(innerTable, pageNumber, false);

                        if (doc.NewPage())
                        {
                            pageNumber++;
                            innerTable = GetDetailPageTable();
                            bc = itemColor;
                        }
                    }

                    innerTable.AddCell(DetailItemCell(item.ManufacturerPN, bc));
                    innerTable.AddCell(DetailItemCell(item.Description, bc));
                    innerTable.AddCell(DetailItemCell(item.Quantity.ToString(), bc, Element.ALIGN_CENTER));
                    innerTable.AddCell(DetailItemCell(item.GetUnitPrice().ToString("C"), bc, Element.ALIGN_RIGHT));
                    innerTable.AddCell(DetailItemCell((item.Quantity * item.GetUnitPrice()).ToString("C"), bc, Element.ALIGN_RIGHT));

                    if (bc == itemColor)
                        bc = altItemColor;
                    else
                        bc = itemColor;

                    index++;
                }

                var ph = (pageNumber == 0) ? firstPageHeight : secondPageHeight;

                if (innerTable.TotalHeight < ph)
                {
                    //need an extra row so that any empty space is white (altItemColor)
                    //also this will push the last row to the top, otherwise it's middle aligned in the empty space
                    innerTable.AddCell(DetailItemCell("", altItemColor));
                    innerTable.AddCell(DetailItemCell("", altItemColor));
                    innerTable.AddCell(DetailItemCell("", altItemColor));
                    innerTable.AddCell(DetailItemCell("", altItemColor));
                    innerTable.AddCell(DetailItemCell("", altItemColor));
                }

                //add the last table
                appendOuterTable(innerTable, pageNumber, true);
            }
        }

        private static PdfPTable GetDetailPageTable()
        {
            PdfPTable table = new PdfPTable(5)
            {
                LockedWidth = true
            };

            float width = 414;

            table.SetTotalWidth(new float[] { (width * 0.2F), (width * 0.5F), (width * 0.1F), (width * 0.1F), (width * 0.1F) });

            //header row
            table.AddCell(DetailHeaderCell("Part #"));
            table.AddCell(DetailHeaderCell("Description"));
            table.AddCell(DetailHeaderCell("Qty"));
            table.AddCell(DetailHeaderCell("Price"));
            table.AddCell(DetailHeaderCell("Amount"));

            return table;
        }

        private static PdfPTable GetFooter(IStoreOrder order, bool empty = false)
        {
            PdfPTable table = new PdfPTable(1)
            {
                SpacingBefore = 10
            };

            Phrase phrase = new Phrase();
            if (!empty)
            {
                phrase.Add(new Chunk("Total Balance: ", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.BLACK)));
                phrase.Add(new Chunk(StoreOrders.GetTotal(order).ToString("C"), new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, BaseColor.BLACK)));
            }

            table.AddCell(new PdfPCell(phrase)
            {
                Border = AllBorders(),
                Padding = 10,
                FixedHeight = 38
            });

            return table;
        }

        private static Phrase NormalPhrase(string text)
        {
            return new Phrase(text, new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL));
        }

        private static Phrase BoldPhrase(string text)
        {
            return new Phrase(text, new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD));
        }

        private static PdfPCell HeaderCell(string text)
        {
            Phrase phrase = new Phrase(text, new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, new BaseColor(255, 255, 255)));

            return new PdfPCell(phrase)
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                FixedHeight = 16,
                BackgroundColor = new BaseColor(128, 128, 128),
                Colspan = 2
            };
        }

        private static PdfPCell DetailHeaderCell(string text)
        {
            Phrase phrase = new Phrase(text, new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK));

            return new PdfPCell(phrase)
            {
                Border = AllBorders(),
                BorderColor = new BaseColor(220, 220, 220),
                HorizontalAlignment = Element.ALIGN_CENTER,
                FixedHeight = 14,
                BackgroundColor = new BaseColor(220, 220, 220)
            };
        }

        private static PdfPCell DetailItemCell(string text, BaseColor bgclr, int align = Element.ALIGN_LEFT)
        {
            Phrase phrase = new Phrase(text, new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK));

            return new PdfPCell(phrase)
            {
                Border = AllBorders(),
                BorderColor = new BaseColor(220, 220, 220),
                HorizontalAlignment = align,
                VerticalAlignment = Rectangle.ALIGN_MIDDLE,
                FixedHeight = 28,
                BackgroundColor = bgclr,
                NoWrap = false
            };
        }

        private static int AllBorders()
        {
            return Rectangle.TOP_BORDER + Rectangle.RIGHT_BORDER + Rectangle.BOTTOM_BORDER + Rectangle.LEFT_BORDER;
        }

        private static string GetOrderType(IStoreOrder order)
        {
            //need some way to tell if order is kit or regular
            bool isKit = false;

            return isKit ? "Kit" : "Regular";
        }
    }
}