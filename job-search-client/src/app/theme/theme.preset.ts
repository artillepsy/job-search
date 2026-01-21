import Aura from '@primeuix/themes/aura';
import { definePreset, palette } from '@primeuix/themes';
import { ButtonPreset } from './components/button.preset';
import { ScrollPanelPreset } from './components/scroll-panel.preset';
import { PaginatorPreset } from './components/paginator.preset';

export const CustomThemePreset = definePreset(Aura, {
  primitive: {
    primary: palette('#218cc6'), // Creates {primary.500}, {primary.600}, etc.
    surface: palette('#64748b'), // Standard grays
  },
  semantic: {
    fontFamily: "'Inter', sans-serif",
    fontSize: '1rem',

    primary: '{primary}',
    colorScheme: {
      light: {
        root: {
          primary: {
            color: '{primary.500}',
            contrastColor: '#ffffff'
          }
        }
      }
    }
  },
  components: {
    button: ButtonPreset,
    scrollpanel: ScrollPanelPreset,
    paginator: PaginatorPreset,
  },
});
