import { Typography } from '@mui/material';

export function PlaceholderPage({ title }: { title: string }) {
  return (
    <>
      <Typography variant="h4" component="h1" gutterBottom>
        {title}
      </Typography>
      <Typography color="text.secondary">Nothing here yet.</Typography>
    </>
  );
}
