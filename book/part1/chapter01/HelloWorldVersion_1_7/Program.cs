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
 * Creates a Hello World in PDF version 1.7
 */
namespace HelloWorldVersion_1_7
{
    class HelloWorldVersion_1_7
    {
        /** Path to the resulting PDF file. */
        public const String RESULT = "../../../../../results/part1/chapter01/hello_1_7.pdf";

        /**
         * Creates a PDF file: hello.pdf
         * @param    args    no arguments needed
         */
        static void Main(String[] args)
        {
            // step 1
            Document document = new Document();
            // step 2
            // Creating a PDF 1.7 document
            PdfWriter writer = PdfWriter.GetInstance(document, File.Create(RESULT));
            writer.PdfVersion = PdfWriter.VERSION_1_7;
            // step 3
            document.Open();
            // step 4
            document.Add(new Paragraph("Hello World!"));
            // step 5
            document.Close();
        }
    }
}
