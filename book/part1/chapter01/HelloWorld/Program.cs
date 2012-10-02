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
namespace HelloWorld
{
    class HelloWorld
    {
        /** Path to the resulting PDF file. */
        public const String RESULT = "../../../../../results/part1/chapter01/hello.pdf";

        /**
         * Creates a PDF file: hello.pdf
         * @param    args    no arguments needed
         */

        static void Main(String[] args) {
            new HelloWorld().CreatePdf(RESULT);
        }

        /**
         * Creates a PDF document.
         * @param filename the path to the new PDF document
         */
        public void CreatePdf(String filename) {
            // step 1
            Document document = new Document();
            // step 2
            PdfWriter.GetInstance(document, File.Create(filename));
            // step 3
            document.Open();
            // step 4
            document.Add(new Paragraph("Hello World!"));
            // step 5
            document.Close();
        }
    }
}
