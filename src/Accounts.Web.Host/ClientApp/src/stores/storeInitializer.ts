import RoleStore from './roleStore';
import TenantStore from './tenantStore';
import UserStore from './userStore';
import SessionStore from './sessionStore';
import AuthenticationStore from './authenticationStore';
import AccountStore from './accountStore';
import HourLogEntryStore from './hourLogEntryStore';
import ProjectStore from './projectStore';
import TimesheetStore from './timesheetStore';

export class RootStore {
  authenticationStore: AuthenticationStore;
  roleStore: RoleStore;
  tenantStore: TenantStore;
  userStore: UserStore;
  sessionStore: SessionStore;
  accountStore: AccountStore;
  hourLogEntryStore: HourLogEntryStore;
  projectStore: ProjectStore;
  timesheetStore: TimesheetStore;

  constructor() {
    this.authenticationStore = new AuthenticationStore();
    this.roleStore = new RoleStore();
    this.tenantStore = new TenantStore();
    this.userStore = new UserStore();
    this.sessionStore = new SessionStore();
    this.accountStore = new AccountStore();
    this.hourLogEntryStore = new HourLogEntryStore();
    this.projectStore = new ProjectStore();
    this.timesheetStore = new TimesheetStore(this);
  }
}
