using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ServiceQuotes.API.Models;

namespace ServiceQuotes.API.Helpers;

public class InvoiceDocumentTemplate : IDocument
{
    public Invoice Model { get; }

    public InvoiceDocumentTemplate(Invoice model)
    {
        Model = model;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(50);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
    }

    void ComposeHeader(IContainer container)
    {
        var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text($"Orçamento #{Model.InvoiceNumber:D8}").Style(titleStyle);

                column.Item().Text(text =>
                {
                    text.Span("Data do orçamento: ").SemiBold();
                    text.Span($"{Model.CreatedAt:d}");
                });
            });

            row.ConstantItem(100).Height(50).Placeholder();
        });
    }

    void ComposeContent(IContainer container)
    {
        container
            .PaddingVertical(40).Column(column =>
            {
                column.Spacing(5);

                column.Item().Element(ComposeAddress);

                column.Item().Element(ComposeTable);

                var totalPrice = Model.Items?.Sum(x => x.Price * x.Quantity);
                column.Item().AlignRight().Text($"Total do orçamento: {totalPrice:C2}").FontSize(14);

                column.Item().PaddingTop(25).Element(ComposeComments);
            });
    }

    void ComposeAddress(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(2);

            column.Item().BorderBottom(1).PaddingBottom(5).Text("Cliente: ").SemiBold();

            column.Item().Text(text =>
            {

                text.Span("Nome: ").SemiBold();
                text.Span(Model.CustomerInfo?.Name);
            });

            column.Item().Text(text =>
            {
                text.Span("Telefone: ").SemiBold();
                text.Span(Model.CustomerInfo?.Phone);
            });
        });
    }

    void ComposeTable(IContainer container)
    {
        container.Table(table =>
        {
            // step 1
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(25);
                columns.RelativeColumn(3);
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            // step 2
            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("#");
                header.Cell().Element(CellStyle).Text("Produto");
                header.Cell().Element(CellStyle).AlignRight().Text("Preço Unitário");
                header.Cell().Element(CellStyle).AlignRight().Text("Quantidade");
                header.Cell().Element(CellStyle).AlignRight().Text("Total");

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                }
            });

            // step 3
            foreach (var item in Model.Items!)
            {
                table.Cell().Element(CellStyle).Text($"{Model.Items.IndexOf(item) + 1}");
                table.Cell().Element(CellStyle).Text(item.Name);
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.Price:C2}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantity}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.Price * item.Quantity:C2}");

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                }
            }
        });
    }

    void ComposeComments(IContainer container)
    {
        container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
        {
            column.Spacing(5);
            column.Item().Text("Observações:").FontSize(14);
            column.Item().Text("Esse orçamento é apenas demonstrativo, baseado nos preços procurados na data do documento descrit acima em sites de e-commerce e / ou fornecedores próprios. Sinta-se a vontade para procurar o(s) produto(s) descrito(s) nessa lista em outros lugares a sua escolha.");
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(x =>
        {
            x.CurrentPageNumber();
            x.Span("/");
            x.TotalPages();
        });
    }

}
