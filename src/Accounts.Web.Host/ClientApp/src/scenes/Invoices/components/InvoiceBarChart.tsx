import React, { useEffect, useState } from 'react';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Brush } from 'recharts';
import { Row, Col, Select } from 'antd';
import styled from '@emotion/styled';
import moment from 'moment';

const { Option } = Select;
const StyledSelect = styled(Select)`
  width: 200px;
`;

type InvoiceBarChartProps = {
  data: any;
  loading: boolean;
};

const InvoiceBarChart: React.FC<InvoiceBarChartProps> = ({ data, loading }) => {
  let yearList: any[] = [];
  const [invoiceData, setInvoiceData] = useState<any>();
  const [selectedYear, setSelectedYear] = useState<any>('All Years');
  const initialStartIndex: number = data && data.length > 12 ? data.length - 12 : 0;
  const initialEndIndex: number = data ? data.length - 1 : 0;
  const [startIndex, setStartIndex] = useState<number>(initialStartIndex);
  const [endIndex, setEndIndex] = useState<number>(initialEndIndex);

  useEffect(() => {
    setInvoiceData(data);
    setSelectedYear('All Years');
  }, [data]);

  if (data) {
    const yearArray = data.map((item: any, index: any) => (item = data[index].year));
    yearList = yearArray.reduce((unique: any, item: any) => (unique.includes(item) ? unique : [...unique, item]), []);
    yearList = ['All Years', ...yearList];
  }
  const handleYearChange = (value: any) => {
    if (data) {
      if (!value || value === 'All Years') {
        setSelectedYear("All Years");
        setInvoiceData(data);
        if (data.length > 12) {
          setStartIndex(data.length - 12);
          setEndIndex(data.length - 1);
        }
      } else {
        const yearBasedArray = data.filter((obj: any) => obj.year === value);
        setInvoiceData(yearBasedArray);
        setStartIndex(0);
        setEndIndex(0);
        setSelectedYear(value);
      }
    }
  };

  const CustomTooltip = ({ payload, active, label }: any) => {
    if (active) {
      if (payload !== undefined && payload !== null) {
        const year = payload[0].payload.year;
        const amount = payload[0].payload.monthAmount;
        return (
          <div className="custom-tooltip" style={{ background: 'rgba(255, 255, 255, 0.7)', color: 'black', padding: '10px', borderRadius: '10px' }}>
            <p className="label">{`Month : ${moment()
              .month(label - 1)
              .format('MMMM')}`}</p>
            <p className="label">{`Year : ${moment().year(year).format('YYYY')}`}</p>
            <p className="intro">{`Amount : $ ${amount.toLocaleString()}`}</p>
          </div>
        );
      } else {
        return null;
      }
    }
    return null;
  };
  return (
    <>
      {data && (
        <>
          <Row>
            <Col offset={21}>
              <StyledSelect showSearch allowClear value={selectedYear} placeholder="Select a year" onChange={handleYearChange}>
                {yearList.map((item: any, index: any) => (
                  <Option value={item} key={index}>
                    {item}
                  </Option>
                ))}
              </StyledSelect>
            </Col>
          </Row>
          <ResponsiveContainer width="100%" height={250}>
            <BarChart width={900} height={250} data={invoiceData} margin={{ top: 20, bottom: 20 }}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis
                dataKey="monthName"
                tickFormatter={(label) =>
                  `${moment()
                    .month(label - 1)
                    .format('MMM')}`
                }
              />
              <YAxis yAxisId="left" orientation="left" stroke="#748AA1" />
              <Tooltip content={<CustomTooltip />} />
              <Bar yAxisId="left" dataKey="monthAmount" fill="#1C3FAA" barSize={70} />
              <Brush width={1480} startIndex={startIndex} endIndex={endIndex} dataKey="monthName" stroke="#6587f0" height={30} />
            </BarChart>
          </ResponsiveContainer>
        </>
      )}
    </>
  );
};

export default InvoiceBarChart;
