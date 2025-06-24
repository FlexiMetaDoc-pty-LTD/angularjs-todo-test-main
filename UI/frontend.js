var app = angular.module("todoApp", []);

const apiBaseUrl = "https://localhost:7200/api"; // ✅ correct base path

// 🔐 Login Controller
app.controller("LoginController", function ($scope, $http, $window) {
  $scope.user = {};

  $scope.login = function () {
    $http.post(`${apiBaseUrl}/auth/login`, $scope.user).then(
      function (response) {
        localStorage.setItem("token", response.data.token);
        $window.location.href = "todo.html";
      },
      function (error) {
        $scope.error = error.data.message || "Invalid login.";
      }
    );
  };
});

// 📝 Register Controller
app.controller("RegisterController", function ($scope, $http, $window) {
  $scope.user = {};

  $scope.register = function () {
    $http.post(`${apiBaseUrl}/auth/register`, $scope.user).then(
      function (response) {
        $window.location.href = "login.html";
      },
      function (error) {
        $scope.error = error.data.message || "Registration failed.";
      }
    );
  };
});

// 🔐 Attach token to every request
app.config(function ($httpProvider) {
  $httpProvider.interceptors.push(function () {
    return {
      request: function (config) {
        const token = localStorage.getItem("token");
        if (token) {
          config.headers["Authorization"] = "Bearer " + token;
        }
        return config;
      },
    };
  });
});
