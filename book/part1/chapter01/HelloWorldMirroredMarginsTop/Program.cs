/*
 * This class is part of the book "iText in Action - 2nd Edition"
 * written by Bruno Lowagie (ISBN: 9781935182610)
 * For more info, go to: http://itextpdf.com/examples/
 * This example only works with the AGPL version of iText.
 */

using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

/**
 * Examples with mirrored margins.
 */
namespace HelloWorldMirroredMarginsTop
{
    class HelloWorldMirroredMarginsTop
    {
        /** Path to the resulting PDF file. */
        public const String RESULT = "../../../../../results/part1/chapter01/hello_mirrored_top.pdf";

        /**
         * Creates a PDF file: hello_mirrored_margins.pdf
         * @param    args    no arguments needed
         */
        static void Main(String[] args)
        {
            // step 1
            Document document = new Document();
            // step 2
            PdfWriter.GetInstance(document, File.Create(RESULT));
            // setting page size, margins and mirrered margins
            document.SetPageSize(PageSize.A5);
            document.SetMargins(36, 72, 108, 180);
            document.SetMarginMirroringTopBottom(true);
            // step 3
            document.Open();
            // step 4
            document.Add(new Paragraph(
                "The left margin of this odd page is 36pt (0.5 inch); " +
                "the right margin 72pt (1 inch); " +
                "the top margin 108pt (1.5 inch); " +
                "the bottom margin 180pt (2.5 inch)."));
            Paragraph paragraph = new Paragraph();
            paragraph.Alignment = Element.ALIGN_JUSTIFIED;
            for (int i = 0; i < 20; i++)
            {
                paragraph.Add("Hello World! Hello People! " +
                    "Hello Sky! Hello Sun! Hello Moon! Hello Stars!");
            }
            document.Add(paragraph);
            document.Add(new Paragraph("The top margin 180pt (2.5 inch)."));
            // step 5
            document.Close();
        }
    }
}
