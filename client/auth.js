angular.module('todoApp')
.controller('AuthController', function($scope, $http) {
    const apiUrl = 'https://localhost:5001/api';

    $scope.login = function() {
        $http.post(`${apiUrl}/auth/login`, $scope.loginData)
            .then(res => {
                localStorage.setItem('accessToken', res.data.accessToken);
                localStorage.setItem('refreshToken', res.data.refreshToken);
                $scope.isAuthenticated = true;
                $scope.loadTodos();
            });
    };

    $scope.register = function() {
        $http.post(`${apiUrl}/auth/register`, $scope.loginData)
            .then(() => $scope.login());
    };
});