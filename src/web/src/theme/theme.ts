import { createTheme } from '@mui/material/styles';
import { tokens } from './tokens';

type SchemeTokens = (typeof tokens)['light'] | (typeof tokens)['dark'];

const palette = (t: SchemeTokens) => ({
  primary: { main: t.primary },
  secondary: { main: t.secondary },
  background: { default: t.background, paper: t.surface },
  text: { primary: t.text },
});

export const theme = createTheme({
  cssVariables: { colorSchemeSelector: 'class' },
  colorSchemes: {
    light: { palette: palette(tokens.light) },
    dark: { palette: palette(tokens.dark) },
  },
  shape: { borderRadius: tokens.radius },
  typography: { fontFamily: tokens.fontFamily },
});
