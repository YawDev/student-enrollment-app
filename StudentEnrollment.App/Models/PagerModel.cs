using System;

namespace StudentEnrollment.App.Models
{
    public class PagerModel
    {
        public Guid Id {get; private set;}
        public string RequestId { get; set; }
        public int TotalItems { get; private set; }
        public int CurrentIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
        public int StartRecord { get; private set; }
        public int EndRecord { get; private set; } 
        public string Action { get; set; } ="Details";

        public PagerModel(int totalItems, int currentindex, int pagesize=5)
        {
            TotalItems = totalItems;
            CurrentIndex = currentindex;
            PageSize = pagesize;
            TotalPages = (int)Math.Ceiling((decimal)totalItems/(decimal)pagesize);
            SetPagesAndRecords();
        }

        public PagerModel(Guid id, int totalItems, int currentindex, int pagesize=5)
        {
            TotalItems = totalItems;
            CurrentIndex = currentindex;
            PageSize = pagesize;
            TotalPages = (int)Math.Ceiling((decimal)totalItems/(decimal)pagesize);
            Id = id;
            SetPagesAndRecords();
        }

        public PagerModel(string requestUserId, int totalItems, int currentindex, int pagesize=5)
        {
            TotalItems = totalItems;
            CurrentIndex = currentindex;
            PageSize = pagesize;
            TotalPages = (int)Math.Ceiling((decimal)totalItems/(decimal)pagesize);
            RequestId = requestUserId;
            SetPagesAndRecords();
        }


        public void SetPagesAndRecords()
        {
            int startPage = CurrentIndex - 5;
            int endPage = CurrentIndex + 4;

            if(startPage <=0)
            {
                endPage = endPage-(startPage-1);
                startPage = 1;
            }

            if(endPage > TotalPages)
            {
                endPage = TotalPages;
                if(endPage > 10)
                    startPage = endPage - 9;
            }

            StartRecord = (CurrentIndex-1)*PageSize+1;
            EndRecord = StartRecord -1 + PageSize;

            if(EndRecord > TotalItems)
                EndRecord = TotalItems;

            if(TotalItems == 0)
            {
                StartPage = 0;
                StartRecord = 0;
                CurrentIndex = 0;
                EndRecord = 0;
            }
            else
            {
                StartPage = startPage;
                EndPage  = endPage;
            }
        }
        
    }
}