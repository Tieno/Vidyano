﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Vidyano.ViewModel
{
    public class QueryItemsSource : ObservableCollection<QueryResultItem>, ISupportIncrementalLoading
    {
        internal QueryItemsSource(StoreQuery query)
        {
            Query = query;

            if (query.HasSearched)
            {
                foreach (var item in query)
                    Add(item);
            }
        }

        public StoreQuery Query { get; private set; }

        public bool HasMoreItems
        {
            get { return (!Query.HasNotification || Query.NotificationType != NotificationType.Error) && (Count < Query.TotalItems || !Query.HasSearched); }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return Query.GetItemsAsync(Count, (int)count).ContinueWith(t =>
            {
                if (t.Exception != null)
                    return new LoadMoreItemsResult { Count = 0 };

                foreach (var item in t.Result)
                    Add(item);

                return new LoadMoreItemsResult { Count = (uint)t.Result.Length };
            }, TaskScheduler.FromCurrentSynchronizationContext()).AsAsyncOperation();
        }
    }
}