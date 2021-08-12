using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchCase.Business.ElasticSearchOptions.Abstract
{
    public interface IElasticSearchConfiguration
    {
        string ConnectionString { get; }
        string AuthUserName { get; }
        string AuthPassword { get; }    
    }
}
