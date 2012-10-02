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
 * Creates a PDF with the biggest possible page size.
 */
namespace HelloWorldMaximum
{
    class HelloWorldMaximum
    {
        /** Path to the resulting PDF file. */
        public const String RESULT = "../../../../../results/part1/chapter01/hello_maximum.pdf";

        /**
         * Creates a PDF file: hello_maximum.pdf
         * Important notice: the PDF is valid (in conformance with
         * ISO-32000), but some PDF viewers won't be able to render
         * the PDF correctly due to their own limitations.
         * @param    args    no arguments needed
         */
        static void Main(String[] args)
        {
            // step 1
            // maximum page size
            Document document = new Document(new Rectangle(14400, 14400));
            // step 2
            PdfWriter writer = PdfWriter.GetInstance(document, File.Create(RESULT));
            // changes the user unit
            writer.Userunit = 75000f;
            // step 3
            document.Open();
            // step 4
            document.Add(new Paragraph("Hello World"));
            // step 5
            document.Close();
        }
    }
}
