﻿namespace Web.Framework.UI
{
    public interface ISayfalanabilirModel
    {
        int PageIndex { get; }
        int PageNumber { get; }
        int PageSize { get; }
        int TotalItems { get; }
        int TotalPages { get; }
        int FirstItem { get; }
        int LastItem { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }
    public interface IPagination<T> : ISayfalanabilirModel
    {

    }
}