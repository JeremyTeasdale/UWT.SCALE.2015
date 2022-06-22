using System;
using System.Collections.Generic;
using System.Linq;

namespace BHS.UWT.TPM
{
    public interface IPagedList
    {
        int TotalCount
        {
            get;
            set;
        }
        int TotalPages
        {
            get;
            set;
        }
        int PageIndex
        {
            get;
            set;
        }

        int PageSize
        {
            get;
            set;
        }

        bool IsPreviousPage
        {
            get;
        }

        bool IsNextPage
        {
            get;
        }
    }

    public class PagedList<T> : List<T>, IPagedList
    {
        public PagedList(IQueryable<T> source, int index, int pageSize)
        {
            int total = source.Count();
            this.TotalCount = total;
            this.TotalPages = total / pageSize;
            this.PageSize = pageSize;

            if (this.TotalCount > 0)
            {
                this.TotalPages = (int)Math.Ceiling((double)this.TotalCount / (double)this.PageSize);
            }
            else
            {
                this.TotalPages = 0;
            }

            if (TotalPages > 0 && TotalPages < index)
                index = TotalPages - 1;

            this.PageIndex = index;

            this.AddRange(source.Skip((index - 1) * pageSize).Take(pageSize).ToList());
        }

        public PagedList(IList<T> source, int index, int pageSize)
        {

            int total = source.Count();
            this.TotalCount = total;
            this.PageSize = pageSize;
            this.PageIndex = index;

            if (this.TotalCount > 0)
            {
                this.TotalPages = (int)Math.Ceiling((double)this.TotalCount / (double)this.PageSize);
            }
            else
            {
                this.TotalPages = 0;
            }

            this.AddRange(source.Skip((index - 1) * pageSize).Take(pageSize).ToList());
        }

        public int TotalPages
        {
            get;
            set;
        }

        public int TotalCount
        {
            get;
            set;
        }

        public int PageIndex
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public bool IsPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }

        public bool IsNextPage
        {
            get
            {
                return (PageIndex * PageSize) <= TotalCount;
            }
        }
    }

    public static class Pagination
    {
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int index, int pageSize)
        {
            return new PagedList<T>(source, index, pageSize);
        }

        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int index)
        {
            return new PagedList<T>(source, index, 10);
        }

        public static PagedList<T> ToPagedList<T>(this List<T> source, int index, int pageSize)
        {
            return new PagedList<T>(source, index, pageSize);
        }
    }
}