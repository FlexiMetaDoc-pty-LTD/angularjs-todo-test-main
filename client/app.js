// AngularJS Todo Application
angular.module('todoApp', [])
.controller('TodoController', function($scope, $http, $interval) {
    const apiUrl = 'https://localhost:5103/api';

    $scope.todos = [];
    $scope.newTodo = '';
    $scope.loginData = {};
    $scope.analytics = {};
    $scope.isAuthenticated = !!localStorage.getItem('accessToken');
    $scope.showRefreshModal = false;
    $scope.refreshCountdown = 60;
    $scope.isDarkMode = false;

    $scope.toggleTheme = function() {
        $scope.isDarkMode = !$scope.isDarkMode;
        document.body.classList.toggle('bg-dark');
        document.body.classList.toggle('text-white');
    };

    const getAuthHeaders = () => ({
        headers: { Authorization: `Bearer ${localStorage.getItem('accessToken')}` }
    });

    $scope.loadTodos = function() {
        $http.get(`${apiUrl}/todo`, getAuthHeaders())
            .then(res => $scope.todos = res.data)
            .catch(console.error);
    };

    $scope.addTodo = function() {
        if (!$scope.newTodo) return;
        $http.post(`${apiUrl}/todo`, { text: $scope.newTodo }, getAuthHeaders())
            .then(() => {
                $scope.newTodo = '';
                $scope.loadTodos();
            });
    };

    $scope.updateTodo = function(todo) {
        $http.put(`${apiUrl}/todo/${todo.id}`, todo, getAuthHeaders())
            .then(() => $scope.loadTodos());
    };

    $scope.deleteTodo = function(todo) {
        $http.delete(`${apiUrl}/todo/${todo.id}`, getAuthHeaders())
            .then(() => $scope.loadTodos());
    };

    $scope.getCompletedCount = () => $scope.todos.filter(t => t.completed).length;
    $scope.getRemainingCount = () => $scope.todos.filter(t => !t.completed).length;

    $scope.logout = function() {
        localStorage.clear();
        $scope.isAuthenticated = false;
    };

    $scope.refreshToken = function() {
        const token = localStorage.getItem('refreshToken');
        if (!token) return;

        $http.post(`${apiUrl}/auth/refresh`, { token })
            .then(res => {
                localStorage.setItem('accessToken', res.data.accessToken);
                $scope.showRefreshModal = false;
                $scope.refreshCountdown = 60;
            });
    };

    function countdownAndLogout() {
        const timer = $interval(() => {
            $scope.refreshCountdown--;
            if ($scope.refreshCountdown <= 0) {
                $interval.cancel(timer);
                $scope.logout();
            }
        }, 1000);
    }

    // Check token expiration every 25 mins
    if ($scope.isAuthenticated) {
        setTimeout(() => {
            $scope.showRefreshModal = true;
            $scope.$apply();
            countdownAndLogout();
        }, 25 * 60 * 1000);

        $scope.loadTodos();
    }

    // Load dashboard
    $http.get(`${apiUrl}/analytics`, getAuthHeaders())
        .then(res => {
            $scope.analytics = res.data;
            renderChart(res.data.chartData);
        });

    function renderChart(data) {
        new Chart(document.getElementById('completionChart'), {
            type: 'bar',
            data: {
                labels: data.labels,
                datasets: [{
                    label: 'Completed Tasks',
                    data: data.values,
                    backgroundColor: 'rgba(40, 167, 69, 0.7)'
                }]
            },
            options: {
                responsive: true,
                scales: {
                    y: { beginAtZero: true }
                }
            }
        });
    }
});
