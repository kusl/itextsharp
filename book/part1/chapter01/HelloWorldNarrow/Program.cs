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
 * Hello World: document constructor.
 */
namespace HelloWorldNarrow
{
    class HelloWorldNarrow
    {
        /** Path to the resulting PDF file. */
        public const String RESULT = "../../../../../results/part1/chapter01/hello_narrow.pdf";

        /**
         * Creates a PDF file: hello_narrow.pdf
         * @param    args    no arguments needed
         */
        static void Main(String[] args)
        {
            // step 1
            // Using a custom page size
            Rectangle pagesize = new Rectangle(216f, 720f);
            Document document = new Document(pagesize, 36f, 72f, 108f, 180f);
            // step 2
            PdfWriter.GetInstance(document, File.Create(RESULT));
            // step 3
            document.Open();
            // step 4
            document.Add(new Paragraph(
                "Hello World! Hello People! " +
                "Hello Sky! Hello Sun! Hello Moon! Hello Stars!"));
            // step 5
            document.Close();
        }
    }
}
