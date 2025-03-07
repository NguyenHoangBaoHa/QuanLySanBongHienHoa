import axios from "axios";

const api = axios.create({
  baseURL: "https://localhost:8007/api",
});

// Middleware tự động gắn token vào request
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Hàm xử lý lỗi chung
const handleApiError = (error) => {
  console.error("API Error:", error);
  if (error.response) {
    const { status, data } = error.response;
    if (status === 401) {
      alert("Phiên đăng nhập hết hạn, vui lòng đăng nhập lại.");
      localStorage.removeItem("token");
      window.location.href = "/login";
    }
    throw data || new Error("Có lỗi xảy ra!");
  }
  throw new Error("Không thể kết nối đến server.");
};

const AccountAPI = {
  login: async (email, password) => {
    try {
      const response = await api.post("/Account/login", { email, password });
      const { token, role, username } = response.data;
      localStorage.setItem("token", token);
      localStorage.setItem("role", role);
      localStorage.setItem("username", username);
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  createStaff: async (staffData) => {
    try {
      const response = await api.post("/Account/create-staff", staffData);
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  registerCustomer: async (customerData) => {
    try {
      const response = await api.post("/Account/register-customer", customerData);
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },
};

// ==================== Pitch Type API ====================
const PitchTypeAPI = {
  GetAll: async () => {
    try {
      const response = await api.get("/PitchType");
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  CreatePitchType: async (data) => {
    try {
      const formData = new FormData();
      formData.append("name", data.name);
      formData.append("price", data.price);
      formData.append("limitPerson", data.limitPerson);

      if (data.images && data.images.length > 0) {
        data.images.forEach((image) => {
          formData.append("imageFile", image);
        });
      }

      const response = await api.post("/PitchType", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  DeletePitchType: async (id) => {
    try {
      await api.delete(`/PitchType/${id}`);
      return { success: true };
    } catch (error) {
      throw handleApiError(error);
    }
  },
};

// ==================== Pitch API ====================
const PitchAPI = {
  GetAllPitches: async () => {
    try {
      const response = await api.get("/Pitch");
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  CreatePitch: async (data) => {
    try {
      const response = await api.post("/Pitch", data, {
        headers: { "Content-Type": "application/json" },
      });
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  DeletePitch: async (id) => {
    try {
      await api.delete(`/Pitch/${id}`);
      return { success: true };
    } catch (error) {
      throw handleApiError(error);
    }
  },
};

// ==================== Booking API ====================
const BookingAPI = {
  GetAllBookings: async () => {
    try {
      const response = await api.get("/Booking");
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  // ✅ Cập nhật đúng API lấy lịch theo tuần (đã sửa)
  GetSchedule: async (pitchId, startDate) => {
    try {
      const response = await api.get(`/booking/pitch/${pitchId}/week`, {
        params: { startDate },
      });
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  GetBookingsByCustomer: async () => {
    try {
      const response = await api.get("/Booking/my-bookings");
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  CreateBooking: async (bookingData) => {
    try {
      const response = await api.post("/Booking", bookingData, {
        headers: { "Content-Type": "application/json" },
      });
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  ConfirmReceived: async (id) => {
    try {
      const response = await api.patch(`/Booking/${id}/confirm-received`);
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  CancelBooking: async (id) => {
    try {
      const response = await api.delete(`/Booking/${id}`);
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },

  UpdatePaymentStatus: async (id, paymentStatus) => {
    try {
      const response = await api.patch(
        `/Booking/${id}/update-payment`,
        { paymentStatus },
        { headers: { "Content-Type": "application/json" } }
      );
      return response.data;
    } catch (error) {
      throw handleApiError(error);
    }
  },
};

export { AccountAPI, PitchTypeAPI, PitchAPI, BookingAPI };
