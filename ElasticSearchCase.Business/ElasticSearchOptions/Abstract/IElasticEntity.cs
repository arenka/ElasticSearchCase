﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchCase.Business.ElasticSearchOptions.Abstract
{
    public interface IElasticEntity<TEntityKey>
    {
        TEntityKey Id { get; set; }
    }

}
