﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.IRepository
{
    public interface IMongoBaseRepository<T> where T : class
    {
        void Create(T entity);
        List<T> GetAll();
        T Get(Expression<Func<T, bool>> filter);
        List<T> GetList(Expression<Func<T, bool>> filter);
        void Remove(Expression<Func<T, bool>> filter);
        void Update(Expression<Func<T, bool>> filter, T entity);
    }

}
