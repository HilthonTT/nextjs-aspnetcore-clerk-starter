/** @type {import('postcss-load-config').Config} */
const config = {
  plugins: {
    // Tailwind CSS v4 ships as a single PostCSS plugin; vendor prefixing is built in,
    // so autoprefixer is no longer needed.
    "@tailwindcss/postcss": {},
  },
};

export default config;
