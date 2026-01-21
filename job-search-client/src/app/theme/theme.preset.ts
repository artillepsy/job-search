import Aura from '@primeuix/themes/aura';
import { definePreset, palette } from '@primeuix/themes';
import { ButtonPreset } from './components/button.preset';

export const CustomThemePreset = definePreset(Aura, {
  primitive: {
    primary: palette('#c82222'), // Creates {primary.500}, {primary.600}, etc. // make darker
    surface: palette('#64748b'), // Standard grays
    success: palette('#00ff88'), // Use for your green buttons
  },
  semantic: {
    fontFamily: "'Inter', sans-serif",
    fontSize: '1rem',

    primary: '{primary}', // Links semantic primary to the primitive palette
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
    button: ButtonPreset
  },
});
