const rules = {
  name: [{ required: true, message: 'Please input your name!' }],
  surname: [{ required: true, message: 'Please input your surname!' }],
  userName: [{ required: true, message: 'Please input your username!' }],
  emailAddress: [{ required: true, message: 'Please input your email!' }],
  role: [{ required: true, message: 'Please specify the role for the user!' }],
};

export default rules;
