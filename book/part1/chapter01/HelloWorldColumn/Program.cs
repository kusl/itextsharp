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
 * First iText example: Hello World.
 */
namespace HelloWorldColumn
{
    class HelloWorldColumn
    {
        /** Path to the resulting PDF file. */
        public const String RESULT = "../../../../../results/part1/chapter01/hello_column.pdf";

        /**
         * Creates a PDF file: hello_column.pdf
         * @param args no arguments needed
         */
        static void Main(String[] args)
        {
            // step 1
            Document document = new Document();
            // step 2
            PdfWriter writer = PdfWriter.GetInstance(document, File.Create(RESULT));
            // step 3
            document.Open();
            // step 4
            // we set the compression to 0 so that we can read the PDF syntax
            writer.CompressionLevel = 0;
            // writes something to the direct content using a convenience method
            Phrase hello = new Phrase("Hello World");
            PdfContentByte canvas = writer.DirectContentUnder;
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, hello, 36, 788, 0);
            // step 5
            document.Close();
        }
    }
}
