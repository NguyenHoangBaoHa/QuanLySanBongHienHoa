import axios from 'axios';
import { jwtDecode } from 'jwt-decode';

const api = axios.create({
  baseURL: 'https://localhost:8007/api',
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
}, (error) => Promise.reject(error));

// Kiểm tra role người dùng (Admin)
// const isAdmin = () => {
//   try {
//     const token = localStorage.getItem('token');
//     if (!token) return false;

//     const payload = jwtDecode(token); // Decode token payload
//     const role = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
//     return role === 'Admin';
//   } catch (error) {
//     console.error('Error decoding token:', error);
//     return false;
//   }
// };

const createPitchTypeFormData = (data) => {
  const formData = new FormData();
  formData.append('name', data.name);
  formData.append('price', data.price);
  formData.append('limitPerson', data.limitPerson);
  if (data.image && data.image.length > 0) {
    data.image.forEach((image) => formData.append("imageFiles", image));
  }
  return formData;
};

const AccountAPI = {
  login: async (email, password) => {
    try {
      const response = await api.post('/Account/login', { email, password });
      const { token, role, username } = response.data;

      // Lưu thông tin vào localStorage
      localStorage.setItem('token', token);
      localStorage.setItem('role', role);
      localStorage.setItem('username', username);

      console.log("Username: ", username + "\nToken: ", token + "\nRole: ", role);

      return response.data;
    } catch (error) {
      console.error('Login failed:', error);
      throw error.response?.data || new Error('Login failed');
    }
  },

  createStaff: async (staffData) => {
    try {
      const response = await api.post('/Account/create-staff', staffData);
      return response.data;
    } catch (error) {
      console.error('Failed to create staff:', error);
      throw error.response?.data || new Error('Failed to create staff');
    }
  },

  registerCustomer: async (customerData) => {
    try {
      const response = await api.post('/Account/register-customer', customerData);
      return response.data;
    } catch (error) {
      console.error('Failed to register customer:', error);
      throw error.response?.data || new Error('Failed to register customer');
    }
  },
};

// PitchTypeAPI with fields: Name, Price, LimitPerson, Image
const PitchTypeAPI = {
  GetAll: async () => {
    try {
      const response = await api.get('/PitchType');
      return response.data.map(pt => ({
        ...pt,
        images: pt.images ? pt.images.map(img => ({ url: `${img}` })) : []
      }));
    } catch (error) {
      console.error('Error fetching pitch types:', error);
      throw error.response?.data || new Error('Unable to fetch pitch types.');
    }
  },

  CreatePitchType: async (data) => {
    try {
      const formData = new FormData();
      formData.append('name', data.name);
      formData.append('price', data.price);
      formData.append('limitPerson', data.limitPerson);

      if (data.images && data.images.length > 0) {
        data.images.forEach((image) => {
          formData.append('imageFile', image);
        });
      }

      const response = await api.post('/PitchType', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });
      return response.data;
    } catch (error) {
      console.error('Error creating pitch type:', error);
      throw error.response?.data || new Error('Unable to create pitch type.');
    }
  },

  UpdatePitchType: async (id, data) => {
    try {
      const formData = new FormData();
      formData.append('name', data.name);
      formData.append('price', data.price);
      formData.append('limitPerson', data.limitPerson);

      if (data.images && data.images.length > 0) {
        data.images.forEach((image) => {
          formData.append('imageFile', image);
        });
      }

      const response = await api.put(`/PitchType/${id}`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });

      return response.data;
    } catch (error) {
      console.error('Error updating pitch type:', error);
      throw error.response?.data || new Error('Unable to update pitch type.');
    }
  },

  DeletePitchType: async (id) => {
    try {
      await api.delete(`/PitchType/${id}`);
      return { success: true };
    } catch (error) {
      console.error('Error deleting pitch type:', error);
      throw error.response?.data || new Error('Unable to delete pitch type.');
    }
  },
};

const PitchAPI = {
  // Lấy danh sách sân
  GetAllPitches: async () => {
    try {
      const response = await api.get('/Pitch');
      return response.data;
    } catch (err) {
      console.error('Lỗi khi lấy danh sách sân: ', err);
      throw err.response?.data || new Error('Không thể lấy danh sách sân.');
    }
  },

  // Thêm mới sân
  CreatePitch: async (data) => {
    try {
      const response = await api.post('/Pitch', data);
      return response.data;
    } catch (error) {
      console.error('Lỗi khi thêm sân:', error);
      throw error.response?.data || new Error('Không thể thêm sân.');
    }
  },

  // Cập nhật sân
  UpdatePitch: async (id, data) => {
    try {
      const response = await api.put(`/Pitch/${id}`, data);
      return response.data;
    } catch (error) {
      console.error('Lỗi khi cập nhật sân:', error);
      throw error.response?.data || new Error('Không thể cập nhật sân.');
    }
  },

  // Xóa sân
  DeletePitch: async (id) => {
    try {
      await api.delete(`/Pitch/${id}`);
      return { success: true };
    } catch (error) {
      console.error('Lỗi khi xóa sân:', error);
      throw error.response?.data || new Error('Không thể xóa sân.');
    }
  },
};

const BookingAPI = {
  // 📌 Lấy danh sách Booking (Admin & Staff)
  GetAllBookings: async () => {
    try {
      const response = await api.get('/Booking');
      return response.data;
    } catch (err) {
      console.error('Lỗi khi lấy danh sách booking: ', err);
      throw err.response?.data || new Error('Không thể lấy danh sách booking.');
    }
  },

  // 📌 Lấy danh sách Booking của chính khách hàng (Customer)
  GetBookingsByCustomer: async () => {
    try {
      const response = await api.get('/Booking/my-bookings');
      return response.data;
    } catch (err) {
      console.error('Lỗi khi lấy danh sách booking của khách hàng: ', err);
      throw err.response?.data || new Error('Không thể lấy danh sách booking của bạn.');
    }
  },

  // 📌 Lấy chi tiết một Booking theo ID
  GetBookingById: async (id) => {
    try {
      const response = await api.get(`/Booking/${id}`);
      return response.data;
    } catch (err) {
      console.error('Lỗi khi lấy chi tiết booking: ', err);
      throw err.response?.data || new Error('Không thể lấy thông tin booking.');
    }
  },

  // 📌 Lấy lịch đặt sân theo tuần (Customer)
  GetBookingScheduleByPitch: async (pitchId, weekStartDate) => {
    try {
      const response = await api.get(`/Booking/schedule/${pitchId}`, {
        params: { weekStartDate },
      });
      return response.data;
    } catch (err) {
      console.error('Lỗi khi lấy lịch đặt sân: ', err);
      throw err.response?.data || new Error('Không thể lấy lịch đặt sân.');
    }
  },

  // 📌 Tạo mới một Booking (Customer)
  CreateBooking: async (bookingData) => {
    try {
      const response = await api.post('/Booking', bookingData);
      return response.data;
    } catch (err) {
      console.error('Lỗi khi tạo booking: ', err);
      throw err.response?.data || new Error('Không thể tạo booking.');
    }
  },

  // 📌 Xác nhận "Nhận sân" (chỉ Staff)
  ConfirmReceived: async (id) => {
    try {
      const response = await api.patch(`/Booking/${id}/confirm-received`);
      return response.data;
    } catch (err) {
      console.error('Lỗi khi xác nhận nhận sân: ', err);
      throw err.response?.data || new Error('Không thể xác nhận nhận sân.');
    }
  },

  // 📌 Hủy Booking (Customer)
  CancelBooking: async (id) => {
    try {
      const response = await api.delete(`/Booking/${id}`);
      return response.data;
    } catch (err) {
      console.error('Lỗi khi hủy booking: ', err);
      throw err.response?.data || new Error('Không thể hủy booking.');
    }
  },

  // 📌 Cập nhật trạng thái thanh toán (Admin & Staff)
  UpdatePaymentStatus: async (id, paymentStatus) => {
    try {
      const response = await api.patch(`/Booking/${id}/update-payment`, { paymentStatus });
      return response.data;
    } catch (err) {
      console.error('Lỗi khi cập nhật trạng thái thanh toán: ', err);
      throw err.response?.data || new Error('Không thể cập nhật trạng thái thanh toán.');
    }
  }
};


export { AccountAPI, PitchTypeAPI, PitchAPI, BookingAPI };