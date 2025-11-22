/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Pages/**/*.cshtml',
        "./Pages/Shared/*.cshtml",
        "./*cshtml"
        
    ],
  theme: {
      extend: {
          backgroundImage: {
              'custom-teal-gradient': 'linear-gradient(to right, #85ad74, #005540)',
          },
          colors: {
              'greens': "#000000",
              'brand-teal': '#85ad74',
              'brand-dark-teal': '#005540',
          }

      },
  },
  plugins: [],
}

