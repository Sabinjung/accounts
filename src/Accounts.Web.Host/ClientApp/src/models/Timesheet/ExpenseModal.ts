import {  observable } from 'mobx';
class ExpenseModel {
     @observable expenseTypeId!: number;
     @observable reportDt!: Date;
     @observable comment!: string;
     @observable amount!: number;
}

export default ExpenseModel