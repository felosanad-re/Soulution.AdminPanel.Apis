using System.Runtime.Serialization;

namespace AdminPanel.Core.Entities.Products
{
    public enum StockType
    {
        [EnumMember(Value ="In Stock")]
        InStock =1,
        [EnumMember(Value ="Low Stock")]
        LowStock,
        [EnumMember(Value ="Out Of Stock")]
        OutOfStock
    }
}
