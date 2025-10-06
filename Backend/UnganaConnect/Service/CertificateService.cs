using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace UnganaConnect.Service
{
    public class CertificateService
    {
        public byte[] GenerateCertificatePdf(string userId, string userName, string courseTitle, DateTime issuedOn)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var bytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(16));

                    page.Header().AlignCenter().Text("Certificate of Completion").FontSize(28).Bold();

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        col.Item().PaddingTop(20).AlignCenter().Text($"This certifies that");
                        col.Item().PaddingTop(10).AlignCenter().Text(userName).FontSize(22).Bold();
                        col.Item().PaddingTop(10).AlignCenter().Text($"has successfully completed");
                        col.Item().PaddingTop(10).AlignCenter().Text(courseTitle).FontSize(20);
                        col.Item().PaddingTop(20).AlignCenter().Text($"Issued on: {issuedOn:MMMM dd, yyyy}");
                        col.Item().PaddingTop(10).AlignCenter().Text($"User ID: {userId}").FontColor(Colors.Grey.Medium);
                    });

                    page.Footer().AlignRight().Text("UnganaConnect");
                });
            }).GeneratePdf();

            return bytes;
        }
    }
}


