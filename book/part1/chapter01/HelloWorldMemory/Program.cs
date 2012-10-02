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
 * Creates a PDF file in memory.
 */
namespace HelloWorldMemory
{
    class HelloWorldMemory
    {
        /** Path to the resulting PDF file. */
        public const String RESULT = "../../../../../results/part1/chapter01/hello_memory.pdf";

        /**
         * Creates a PDF file: hello_memory.pdf
         * @param    args    no arguments needed
         */
        static void Main(String[] args)
        {
            // step 1
            Document document = new Document();
            // step 2
            // we'll create the file in memory
            MemoryStream baos = new MemoryStream();
            PdfWriter.GetInstance(document, baos);
            // step 3
            document.Open();
            // step 4
            document.Add(new Paragraph("Hello World!"));
            // step 5
            document.Close();
            // let's write the file in memory to a file anyway
            FileStream fos = File.Create(RESULT);
            byte[] bytes = baos.ToArray();
            fos.Write(bytes, 0, bytes.Length);
            fos.Close();
        }
    }
}
