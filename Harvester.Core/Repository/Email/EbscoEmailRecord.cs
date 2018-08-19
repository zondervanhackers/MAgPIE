using System;
using System.Diagnostics.CodeAnalysis;
using FileHelpers;
using ZondervanLibrary.Harvester.Core.Repository.Counter;
// ReSharper disable UnassignedField.Global
// ReSharper disable once ClassNeverInstantiated.Global

#pragma warning disable 649
#pragma warning disable 169
namespace ZondervanLibrary.Harvester.Core.Repository.Email
{
    [DelimitedRecord("\t")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
    public class EbscoEmailRecord
    {
        [FieldQuoted]
        [FieldOrder(10)]
        public string Title;

        [FieldValueDiscarded]
        [FieldOrder(20)]
        public string Publisher;

        [FieldQuoted]
        [FieldConverter(typeof(ItemTypeConverter))]
        [FieldOrder(30)]
        public ItemType ResourceType;

        [FieldValueDiscarded]
        [FieldOrder(40)]
        public string Subject;

        [FieldQuoted]
        [FieldOrder(50)]
        public string Proprietary_Identifier;

        [FieldQuoted]
        [FieldOrder(60)]
        public string Print_ISSN;

        [FieldQuoted]
        [FieldOrder(70)]
        public string Online_ISSN;

        [FieldQuoted]
        [FieldOrder(80)]
        public string ISBN;

        [FieldConverter(ConverterKind.Int32)]
        [FieldOrder(90)]
        [FieldQuoted(QuoteMode.OptionalForBoth)]
        [FieldNullValue(0)]
        public int Total_Requests;

        [FieldConverter(ConverterKind.Int32)]
        [FieldOrder(100)]
        [FieldQuoted(QuoteMode.OptionalForBoth)]
        [FieldNullValue(0)]
        public int Total_Full_Text_Requests;

        [FieldConverter(ConverterKind.Int32)]
        [FieldOrder(110)]
        [FieldQuoted(QuoteMode.OptionalForBoth)]
        [FieldNullValue(0)]
        public int Total_LinkOut_Requests;

        [FieldConverter(ConverterKind.Int32)]
        [FieldOrder(120)]
        [FieldQuoted(QuoteMode.OptionalForBoth)]
        [FieldNullValue(0)]
        public int Abstract_Requests;

        [FieldConverter(ConverterKind.Int32)]
        [FieldOrder(130)]
        [FieldQuoted(QuoteMode.OptionalForBoth)]
        [FieldNullValue(0)]
        public int Turnaways;

        [FieldConverter(ConverterKind.Int32)]
        [FieldOrder(140)]
        [FieldOptional]
        [FieldQuoted(QuoteMode.OptionalForBoth)]
        [FieldNullValue(0)]
        public int Html_Full_Text_Requests;

        [FieldConverter(ConverterKind.Int32)]
        [FieldOrder(150)]
        [FieldOptional]
        [FieldQuoted(QuoteMode.OptionalForBoth)]
        [FieldNullValue(0)]
        public int Pdf_Full_Text_Requests;

        [FieldConverter(ConverterKind.Int32)]
        [FieldOrder(160)]
        [FieldOptional]
        [FieldQuoted(QuoteMode.OptionalForBoth)]
        [FieldNullValue(0)]
        public int Multimedia_Requests;

        [FieldConverter(ConverterKind.Int32)]
        [FieldOrder(170)]
        [FieldOptional]
        [FieldQuoted(QuoteMode.OptionalForBoth)]
        [FieldNullValue(0)]
        public int SmartLink_Linkout_Requests;

        [FieldConverter(ConverterKind.Int32)]
        [FieldOrder(180)]
        [FieldOptional]
        [FieldQuoted(QuoteMode.OptionalForBoth)]
        [FieldNullValue(0)]
        public int CustomLink_Linkout_Requests;
    }

    public class EbscoBookEmailRecord : EbscoEmailRecord
    {
        [FieldQuoted]
        [FieldOrder(55)]
        public string Book_ID;

        [FieldConverter(ConverterKind.Int32)]
        [FieldOrder(155)]
        [FieldOptional]
        [FieldQuoted(QuoteMode.OptionalForBoth)]
        [FieldNullValue(0)]
        public int eBook_Online_Requests;
    }

    internal class ItemTypeConverter : ConverterBase
    {
        public override object StringToField(string itemType)
        {
            switch (itemType)
            {
                case "Report":
                    return ItemType.Journal;
                case "Book Series":
                case "Audio Book":
                    return ItemType.Book;
                case "Streaming Audio":
                    return ItemType.StreamingAudio;
            }

            if (Enum.TryParse(itemType, out ItemType result))
                return result;

            throw new ArgumentException($"Failed to parse ItemType with value of {itemType}", nameof(itemType));
        }
    }
}