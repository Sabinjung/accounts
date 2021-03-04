const rules = {
  name: [{ required: true, message: 'Please input your name!' }],
  surname: [{ required: true, message: 'Please input your last name!' }],
  userName: [{ required: true, message: 'Please input your user name!' }],
  emailAddress: [{ required: true, message: 'Please input your email!' }],
  role: [{ required: true, message: 'Please specify the role for the user!' }],
};

export default rules;
