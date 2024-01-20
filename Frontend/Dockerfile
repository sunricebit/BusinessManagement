# Sử dụng một image chứa Node.js
FROM node:14

# Thiết lập thư mục làm việc
WORKDIR /app

# Copy tất cả các tệp từ thư mục hiện tại vào thư mục làm việc
COPY . .

# Cài đặt dependencies
RUN npm install

# Build ứng dụng Angular
RUN npm run build --prod

# Mở cổng cho ứng dụng 
EXPOSE 80

# Khởi động ứng dụng Angular 
CMD ["npm", "start"]
