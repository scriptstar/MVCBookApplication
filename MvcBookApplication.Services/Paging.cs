﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcBookApplication.Services
{
    public interface IPagedList<T> : IList<T>
    {
        int PageCount { get; }
        int TotalItemCount { get; }
        int PageIndex { get; }
        int PageNumber { get; }
        int PageSize { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        bool IsFirstPage { get; }
        bool IsLastPage { get; }
    }

    public class PagedList<T> : List<T>, IPagedList<T>
    {
        public PagedList(IEnumerable<T> source, int index, int pageSize)
        {
            if (source is IQueryable<T>)
                Initialize(source as IQueryable<T>, index, pageSize);
            else
                Initialize(source.AsQueryable(), index, pageSize);
        }

        public PagedList(IQueryable<T> source, int index, int pageSize)
        {
            Initialize(source, index, pageSize);
        }

        #region IPagedList Members

        public int PageCount { get; private set; }
        public int TotalItemCount { get; private set; }
        public int PageIndex { get; private set; }
        public int PageNumber { get { return PageIndex + 1; } }
        public int PageSize { get; private set; }
        public bool HasPreviousPage { get; private set; }
        public bool HasNextPage { get; private set; }
        public bool IsFirstPage { get; private set; }
        public bool IsLastPage { get; private set; }

        #endregion

        protected void Initialize(IQueryable<T> source, int index, int pageSize)
        {
            //### set source to blank list if source is null to prevent exceptions
            if (source == null)
                source = new List<T>().AsQueryable();

            //### set properties
            TotalItemCount = source.Count();
            PageSize = pageSize;
            PageIndex = index;
            if (TotalItemCount > 0)
                PageCount = (int)Math.Ceiling(TotalItemCount / (double)PageSize);
            else
                PageCount = 0;
            HasPreviousPage = (PageIndex > 0);
            HasNextPage = (PageIndex < (PageCount - 1));
            IsFirstPage = (PageIndex <= 0);
            IsLastPage = (PageIndex >= (PageCount - 1));

            //### argument checking
            if (index < 0)
                throw new
                    ArgumentOutOfRangeException("PageIndex cannot be below 0.");
            if (pageSize < 1)
                throw new
                    ArgumentOutOfRangeException("PageSize cannot be less than 1.");

            //### add items to internal list
            if (TotalItemCount > 0)
                AddRange(source.Skip((index) * pageSize).Take(pageSize).ToList());
        }
    }

public static class Pagination
{
    public static PagedList<T> ToPagedList<T>(this IQueryable<T> source,
                                                int index, int pageSize)
    {
        return new PagedList<T>(source, index, pageSize);
    }

    public static PagedList<T> ToPagedList<T>(this IEnumerable<T> source,
                                                int index, int pageSize)
    {
        return new PagedList<T>(source, index, pageSize);
    }
}
}
