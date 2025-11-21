/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Pages/**/*.cshtml',
        
    ],
  theme: {
      extend: {
          backgroundImage: {
              'custom-teal-gradient': 'linear-gradient(to right, #85ad74, #005540)',
          }
      },
  },
  plugins: [],
}

