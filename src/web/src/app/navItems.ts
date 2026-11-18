import type { SvgIconComponent } from '@mui/icons-material';
import InboxIcon from '@mui/icons-material/Inbox';
import AssignmentIcon from '@mui/icons-material/Assignment';
import RocketLaunchIcon from '@mui/icons-material/RocketLaunch';
import MenuBookIcon from '@mui/icons-material/MenuBook';
import DashboardIcon from '@mui/icons-material/Dashboard';
import SettingsIcon from '@mui/icons-material/Settings';

export interface NavItem {
  label: string;
  path: string;
  icon: SvgIconComponent;
}

export const navItems: NavItem[] = [
  { label: 'Inbox', path: '/inbox', icon: InboxIcon },
  { label: 'Tickets', path: '/tickets', icon: AssignmentIcon },
  { label: 'Releases', path: '/releases', icon: RocketLaunchIcon },
  { label: 'Knowledge', path: '/knowledge', icon: MenuBookIcon },
  { label: 'Dashboards', path: '/dashboards', icon: DashboardIcon },
  { label: 'Settings', path: '/settings', icon: SettingsIcon },
];
