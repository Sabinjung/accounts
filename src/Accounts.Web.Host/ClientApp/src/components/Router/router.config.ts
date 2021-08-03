import LoadableComponent from './../Loadable/index';

export const userRouter: any = [
  {
    path: '/user',
    name: 'user',
    title: 'User',
    component: LoadableComponent(() => import('../../components/Layout/UserLayout')),
    isLayout: true,
    showInMenu: false,
  },
  {
    path: '/user/login',
    name: 'login',
    title: 'LogIn',
    component: LoadableComponent(() => import('../../scenes/Login')),
    showInMenu: false,
  },
  {
    path: '/user/oauth_callback',
    name: 'login',
    title: 'LogIn',
    component: LoadableComponent(() => import('../../scenes/Login/OauthCallback')),
    showInMenu: false,
  },
];

export const appRouters: any = [
  {
    path: '/',
    exact: true,
    name: 'home',
    permission: '',
    title: 'Home',
    icon: 'home',
    component: LoadableComponent(() => import('../../components/Layout/AppLayout')),
    isLayout: true,
    showInMenu: false,
  },
  // {
  //   path: '/dashboard',
  //   name: 'dashboard',
  //   permission: '',
  //   title: 'Dashboard',
  //   icon: 'icondashboard',
  //   showInMenu: true,
  //   component: LoadableComponent(() => import('../../scenes/Dashboard')),
  // },
  {
    path: '/hourlogentry',
    name: 'Hour Logs',
    permission: 'Timesheet',
    title: 'Hour Logs',
    icon: 'iconhourlogs',
    showInMenu: true,
    component: LoadableComponent(() => import('../../scenes/Timesheet')),
  },
  {
    path: '/timesheets',
    name: 'Timesheets',
    permission: 'Timesheet',
    title: 'Timesheets',
    icon: 'icontimesheets',
    showInMenu: true,
    component: LoadableComponent(() => import('../../scenes/TimesheetReview')),
  },

  {
    path: '/consultants',
    name: 'Consultants',
    permission: 'Consultant',
    title: 'Consultants',
    icon: 'iconconsultants',
    showInMenu: true,
    component: LoadableComponent(() => import('../../scenes/Consultants')),
  },
  {
    path: '/companies',
    name: 'Companies',
    permission: 'Company',
    title: 'Companies',
    icon: 'iconcompanies',
    showInMenu: true,
    component: LoadableComponent(() => import('../../scenes/Companies')),
  },

  {
    path: '/endClients',
    name: 'EndClinets',
    permission: 'Endclient',
    title: 'End Clients',
    icon: 'iconendclients',
    showInMenu: true,
    exact: true,
    component: LoadableComponent(() => import('../../scenes/EndClients')),
  },

  {
    path: '/projects/:projectId/detail',
    name: 'Project Details',
    permission: 'Project',
    title: 'Project Detail',
    icon: 'project',
    showInMenu: false,
    exact: true,
    component: LoadableComponent(() => import('../../scenes/ProjectDetail')),
  },

  {
    path: '/projects',
    name: 'Projects',
    permission: 'Project',
    title: 'Projects',
    icon: 'iconproject',
    exact: true,
    showInMenu: true,
    component: LoadableComponent(() => import('../../scenes/Projects')),
  },

  {
    path: '/invoices',
    name: 'Invoices',
    permission: 'Project',
    title: 'Invoices',
    icon: 'iconinvoice',
    showInMenu: true,
    exact: true,
    component: LoadableComponent(() => import('../../scenes/Invoices')),
  },

  {
    path: '/hourlogsReport',
    name: 'hourLogsReport',
    permission: 'HourLog.Report',
    title: 'Hour Logs Report',
    icon: 'icona-hourlogsreport',
    showInMenu: true,
    exact: true,
    component: LoadableComponent(() => import('../../scenes/Timesheet/components/HourLogReport')),
  },

  {
    path: '/agingReport',
    name: 'agingReport',
    permission: 'AgingReport',
    title: 'Aging Report',
    icon: 'iconagingreport',
    showInMenu: true,
    exact: true,
    component: LoadableComponent(() => import('../../scenes/AgingReport')),
  },

  {
    path: '/payrollHours',
    name: 'PayrollHours',
    permission: '',
    title: 'Payroll Hours',
    icon: 'iconPayrollhours',
    showInMenu: true,
    exact: true,
    component: LoadableComponent(() => import('../../scenes/PayrollHours')),
  },

  {
    path: '/users',
    permission: 'Pages.Users',
    title: 'Users',
    name: 'user',
    icon: 'iconuser',
    showInMenu: true,
    component: LoadableComponent(() => import('../../scenes/Users')),
  },
  {
    path: '/roles',
    permission: 'Pages.Roles',
    title: 'Roles',
    name: 'role',
    icon: 'iconRoles',
    showInMenu: true,
    component: LoadableComponent(() => import('../../scenes/Roles')),
  },
  {
    path: '/tenants',
    permission: 'Pages.Tenants',
    title: 'Tenants',
    name: 'tenant',
    icon: 'icontenants',
    showInMenu: true,
    component: LoadableComponent(() => import('../../scenes/Tenants')),
  },
  {
    path: '/test',
    permission: 'Pages.Tenants',
    title: 'Tests',
    name: 'test',
    icon: 'icontests',
    showInMenu: true,
    component: LoadableComponent(() => import('../../scenes/Test')),
  },
  {
    path: '/logout',
    permission: '',
    title: 'Logout',
    name: 'logout',
    icon: 'info-circle',
    showInMenu: false,
    component: LoadableComponent(() => import('../../components/Logout')),
  },
  {
    path: '/refresh',
    permission: '',
    title: 'Logout',
    name: 'logout',
    icon: 'info-circle',
    showInMenu: false,
    component: LoadableComponent(() => import('../../components/RefreshAuthentication')),
  },
  {
    path: '/exception',
    permission: '',
    title: 'exception',
    name: 'exception',
    icon: 'info-circle',
    showInMenu: false,
    component: LoadableComponent(() => import('../../scenes/Exception')),
  },
];

export const routers = [...userRouter, ...appRouters];
