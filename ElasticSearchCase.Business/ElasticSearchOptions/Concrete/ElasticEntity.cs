using ElasticSearchCase.Business.ElasticSearchOptions.Abstract;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchCase.Business.ElasticSearchOptions.Concrete
{
    public class ElasticEntity<TEntityKey> : IElasticEntity<TEntityKey>
    {
        public virtual TEntityKey Id { get; set; }
        public virtual CompletionField Suggest { get; set; }
        public virtual string SearchingArea { get; set; }
        public virtual double? Score { get; set; }
    }
}
