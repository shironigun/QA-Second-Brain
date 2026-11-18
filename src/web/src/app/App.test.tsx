import { render, screen, within } from '@testing-library/react';
import { createMemoryRouter, RouterProvider } from 'react-router';
import { AppProviders } from './AppProviders';
import { routes } from './routes';

function renderAt(path: string) {
  const router = createMemoryRouter(routes, { initialEntries: [path] });
  return render(
    <AppProviders>
      <RouterProvider router={router} />
    </AppProviders>,
  );
}

describe('App shell', () => {
  it('renders the app bar with a Sign in button', () => {
    renderAt('/');
    const banner = screen.getByRole('banner');
    expect(within(banner).getByText('QA Second Brain')).toBeInTheDocument();
    expect(within(banner).getByRole('button', { name: 'Sign in' })).toBeInTheDocument();
  });

  it('renders the left nav with every section', () => {
    renderAt('/');
    const nav = screen.getByRole('navigation', { name: 'Main' });
    for (const label of ['Inbox', 'Tickets', 'Releases', 'Knowledge', 'Dashboards', 'Settings']) {
      expect(within(nav).getByRole('link', { name: label })).toBeInTheDocument();
    }
  });

  it('shows the placeholder page for the current route', () => {
    renderAt('/releases');
    expect(screen.getByRole('heading', { level: 1, name: 'Releases' })).toBeInTheDocument();
  });
});
