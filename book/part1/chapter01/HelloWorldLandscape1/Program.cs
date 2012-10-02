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
 * A possible way to create a document in the landscape format.
 */
namespace HelloWorldLandscape1
{
    class HelloWorldLandscape1
    {
        /** Path to the resulting PDF file. */
        public const String RESULT = "../../../../../results/part1/chapter01/hello_landscape1.pdf";

        /**
         * Creates a PDF file: hello_landscape1.pdf
         * @param    args    no arguments needed
         */
        static void Main(String[] args)
        {
            // step 1
            // landscape format using the rotate() method
            Document document = new Document(PageSize.LETTER.Rotate());
            // step 2
            PdfWriter.GetInstance(document, File.Create(RESULT));
            // step 3
            document.Open();
            // step 4
            document.Add(new Paragraph("Hello World"));
            // step 5
            document.Close();
        }
    }
}
