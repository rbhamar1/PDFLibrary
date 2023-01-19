PDF Library

A Library helps to create PDF using millimeters as the unit of measurement. It builds a PDF from a set of logical pages and paginates them with optional headers, footers etc..

Let’s get introduce with different components implemented inside a bunch :

1. Use PdfFontFactory to create fonts used for printing
2. PdfPageTemplate describes the maximum size of the printable page, as well as the margins
3. Use PdfPageTemplateFactory to insulate your code from particulars of the printer
4. PdfPaginator allows the client to append nodes representing output, which are divided into one or more instances of PdfPage. The printer has a maximum height of PDF to print. This means that even print output that is not paginated from the user's point of view (e.g., pick lists) must be paginated using PdfPaginator
5. PdfPaginator supports page headers and footers
6. PdfPaginator supports a fluent interface. Much of our printed output consists either entirely or mostly of text with a single font
7. PdfPaginator has a WithFont method that sets the default font. This is default font getting use, if font is not provided for the text nodes that are added to the paginator
8. Once all of the output has been added, the PdfPaginator.GetPages method can be called to get the pages that resulted IPdfBuilder(which is created using an IPdfBuilderFactory) can then be used to create a PDF
