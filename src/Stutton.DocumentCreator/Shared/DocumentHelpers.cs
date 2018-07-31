using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace Stutton.DocumentCreator.Shared
{
    public static class DocumentHelpers
    {
        private const long EmuPerInch = 914400L;
        private const int TwipsPerInch = 1440;

        private const long DefaultPageWidth = (long)8.5 * TwipsPerInch;
        private const long DefaultMargin = (long)1 * TwipsPerInch;

        private static uint _nextId = 0;

        public static void AddNumberedTextToBody(this WordprocessingDocument wordDoc, string text, int number)
        {
            wordDoc.AddTextToBody($"{number}.\t{text}");
        }

        public static void AddTextToBody(this WordprocessingDocument wordDoc, string text)
        {
            wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(new Text(text))));
        }

        public static void AddImageToBody(this WordprocessingDocument wordDoc, BitmapSource image)
        {
            var mainPart = wordDoc.MainDocumentPart;
            var imagePart = mainPart.AddImagePart(ImagePartType.Png);
            using (var memStream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(memStream);
                memStream.Seek(0, SeekOrigin.Begin);
                imagePart.FeedData(memStream);
            }
            wordDoc.AddImageToBody(mainPart.GetIdOfPart(imagePart), image);
        }

        private static void AddImageToBody(this WordprocessingDocument wordDoc, string relationshipId, BitmapSource image)
        {
            var imageCx = CalculateImageCx(image);
            var imageCy = CalculateImageCy(image);

            var usablePageWidth = CalculateUsableWidthInEmus(wordDoc);

            if (imageCx > usablePageWidth)
            {
                var ratio = (double)usablePageWidth / (double)imageCx;
                imageCx = usablePageWidth;
                imageCy = (long)((double)imageCy * ratio);
            }

            var element = new Drawing(
                new DW.Inline(
                    new DW.Extent { Cx = imageCx, Cy = imageCy },
                    new DW.EffectExtent { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L },
                    new DW.DocProperties { Id = GetNextId(), Name = "Picture 1" },
                    new DW.NonVisualGraphicFrameDrawingProperties(
                        new A.GraphicFrameLocks { NoChangeAspect = true }),
                    new A.Graphic(
                        new A.GraphicData(
                            new PIC.Picture(
                                new PIC.NonVisualPictureProperties(
                                    new PIC.NonVisualDrawingProperties { Id = 0U, Name = "New Bitmap Image.png" },
                                    new PIC.NonVisualPictureDrawingProperties()),
                                new PIC.BlipFill(
                                    new A.Blip(
                                        new A.BlipExtensionList(
                                            new A.BlipExtension() { Uri = $"{{{Guid.NewGuid().ToString()}}}" }
                                            )
                                        )
                                    { Embed = relationshipId, CompressionState = A.BlipCompressionValues.Print },
                                    new A.Stretch(
                                        new A.FillRectangle()
                                        )
                                    ),

                            new PIC.ShapeProperties(
                                new A.Transform2D(
                                    new A.Offset { X = 0L, Y = 0L },
                                    new A.Extents { Cx = imageCx, Cy = imageCy }
                                    ),
                                new A.PresetGeometry(
                                    new A.AdjustValueList()
                                    )
                                { Preset = A.ShapeTypeValues.Rectangle }
                                )
                                )
                            )
                        { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }
                        )
                    )
                {
                    DistanceFromTop = (UInt32Value)0U,
                    DistanceFromBottom = (UInt32Value)0U,
                    DistanceFromLeft = (UInt32Value)0U,
                    DistanceFromRight = (UInt32Value)0U,
                    EditId = "50D07946"
                }
            );
            wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
        }

        private static long CalculateImageCx(BitmapSource image)
        {
            return (long)(image.PixelWidth / image.DpiX * EmuPerInch);
        }

        private static long CalculateImageCy(BitmapSource image)
        {
            return (long)(image.PixelHeight / image.DpiY * EmuPerInch);
        }

        private static long CalculateUsableWidthInEmus(WordprocessingDocument wordDoc)
        {
            var sectionProperties = wordDoc.MainDocumentPart.Document.Body.GetFirstChild<SectionProperties>();
            var pageSize = sectionProperties.GetFirstChild<PageSize>();
            var pageMargins = sectionProperties.GetFirstChild<PageMargin>();

            var pageWidth = pageSize?.Width?.Value ?? DefaultPageWidth;
            var rightMargin = pageMargins?.Right?.Value ?? DefaultMargin;
            var leftMargin = pageMargins?.Left?.Value ?? DefaultMargin;

            var usablePageWidthInTwips = pageWidth - (rightMargin + leftMargin);
            var usablePageWidthInInches = usablePageWidthInTwips / TwipsPerInch;
            var usablePageWidthInEmus = usablePageWidthInInches * EmuPerInch;
            return usablePageWidthInEmus;
        }

        private static UInt32Value GetNextId()
        {
            _nextId++;
            return _nextId;
        }
    }
}
