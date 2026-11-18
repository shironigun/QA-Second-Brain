import { Navigate, type RouteObject } from 'react-router';
import { AppLayout } from './AppLayout';
import { navItems } from './navItems';
import { PlaceholderPage } from '../pages/PlaceholderPage';

export const routes: RouteObject[] = [
  {
    path: '/',
    element: <AppLayout />,
    children: [
      { index: true, element: <Navigate to="/inbox" replace /> },
      ...navItems.map(({ label, path }) => ({ path, element: <PlaceholderPage title={label} /> })),
      { path: '*', element: <PlaceholderPage title="Not found" /> },
    ],
  },
];
