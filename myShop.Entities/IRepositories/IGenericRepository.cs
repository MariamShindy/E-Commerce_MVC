﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace myShop.Entities.Repositories
{
    public interface IGenericRepository<T> where T : class
    {

        IEnumerable<T> GetAll(Expression<Func<T , bool>>? predicate = null ,string? includeWord = null);
        T GetFirstOrDefault(Expression<Func<T, bool>>? predicate = null , string? includeWord = null);
        void Add(T entity);
        void Remove (T entity);
        void RemoveRange (IEnumerable<T> entities);


    }
}
