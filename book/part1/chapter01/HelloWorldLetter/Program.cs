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
 * Hello World example using the paper size Letter.
 */
namespace HelloWorldLetter
{
    class HelloWorldLetter
    {
        /** Path to the resulting PDF file. */
        public const String RESULT = "../../../../../results/part1/chapter01/hello_letter.pdf";

        /**
         * Creates a PDF file: hello_letter.pdf.
         * @param    args    no arguments needed
         */
        static void Main(String[] args)
        {
            // step 1
            // Specifying the page size
            Document document = new Document(PageSize.LETTER);
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
