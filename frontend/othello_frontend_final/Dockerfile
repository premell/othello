# Use Node.js as base image
FROM node:18

# Set working directory in the Docker container
WORKDIR /usr/src/app

# Copy package.json and package-lock.json
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of your app's source code
COPY . .

# Build the SvelteKit app
RUN npm run build

# Expose the port that your app runs on
EXPOSE 3000

# Define the command to run your app
CMD ["node", "build"]
