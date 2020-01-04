﻿using Abp.Application.Services;
using Abp.Domain.Repositories;
using Accounts.Expenses.Dto;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Expenses
{
    public class ExpenseTypeService : AsyncCrudAppService<ExpenseType, ExpenseTypeDto>
    {
        public ExpenseTypeService(IRepository<ExpenseType> repository)
        : base(repository)
        {

        }

    }
}