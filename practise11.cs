public partial class MainWindow : Window
{
    public static int AuthUser = 0;

    List<string> filters = new List<string>() { "Без фильтра", "Таблетки", "Мази", "Настойки", "Микстуры", "Растворы", "Порошки", "Изготовляемые", "Готовые" };
    List<string> sortList = new List<string>() { "По алфавиту", "В обратном порядке", "По цене ↓", "По цене ↑" };
    public MainWindow()
    {
        if (AuthUser == 0)
        {
            Authorisation authorisation = new Authorisation();
            authorisation.ShowDialog();
        }
        InitializeComponent();
        Filterscb.ItemsSource = filters;
        Sortirovkacb.ItemsSource = sortList;
        PharmacyClass.db.Medicament.Load();
        MedicamentsDG.ItemsSource = PharmacyClass.db.Medicament.Local.ToBindingList();
        SetButtonsVisibility(AuthUser);
    }

    private void SetButtonsVisibility(int AuthUser)
    {
        ToAdminbt.Visibility = AuthUser == 1 ? Visibility.Visible : Visibility.Collapsed;
        AddToCartbt.Visibility = AuthUser == 2 ? Visibility.Visible : Visibility.Collapsed;
        ToClientbt.Visibility = AuthUser == 2 ? Visibility.Visible : Visibility.Collapsed;
        ToEmployeebt.Visibility = AuthUser == 3 ? Visibility.Visible : Visibility.Collapsed;
    }


    private void ToAdminbt_Click(object sender, RoutedEventArgs e)
    {
        AdminWindow adminWindow = new AdminWindow();
        adminWindow.Show();
        Close();
    }

    private void ToEmployeebt_Click(object sender, RoutedEventArgs e)
    {
        EmployeeWindow employeeWindow = new EmployeeWindow();
        employeeWindow.Show();
        Close();
    }

    private void ToClientBt_Click(object sender, RoutedEventArgs e)
    {
        ClientWindow clientWindow = new ClientWindow();
        clientWindow.Show();
        Close();
    }

    private void Searchbt_Click(object sender, RoutedEventArgs e)
    {
        MedicamentsDG.ItemsSource = PharmacyClass.db.Medicament.Local.Where(m => m.Medicament_Name.ToLower().Contains(Searchtb.Text.ToLower()));
    }

    /// <summary>
    /// Добавляет выбранный медикамент в корзину клиента
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddToCartBt_Click(object sender, RoutedEventArgs e)
    {
        // Получаем выбранный медикамент из таблицы
        Medicament SelectedMedicament = MedicamentsDG.SelectedItem as Medicament;
        // Проверяем, что медикамент был выбран
        if (SelectedMedicament == null)
        {
            // Выводим сообщение об ошибке
            MessageBox.Show("Для добавления в корзину нужно выбрать медикамент");
            // Прерываем выполнение метода
            return;
        }
        // Добавляем медикамент в корзину клиента
        ClientWindow.medicaments.Add(SelectedMedicament);
    }
}

public partial class ClientWindow : Window
{
    public static BindingList<Medicament> medicaments = new BindingList<Medicament>();
    double OrderPrice;
    public static User authUser;
    public ClientWindow()
    {
        InitializeComponent();
        try
        {
            PharmacyClass.db.Medicament.Load();
        }
        catch
        {
            MessageBox.Show("Не удалось получить данные");
        }
        OrderDictionary.ItemsSource = PharmacyClass.db.Order.Where(u => u.User_ID == authUser.ID_User).ToList();
        CartDB.ItemsSource = medicaments;
    }

    private void ClientToMainbt_Click(object sender, RoutedEventArgs e)
    {
        MainWindow main = new MainWindow();
        main.Show();
        Close();
    }

    private void Orderbt_Click(object sender, RoutedEventArgs e)
    {
        if (medicaments.Count == 0)
        {
            MessageBox.Show("Нельзя сделать заказ, пока вы ничего не добавили в корзину", "Ошибка");
            return;
        }
        foreach (Medicament item in medicaments)
        {
            OrderPrice += Convert.ToDouble(item.Medicament_Price);
        }
        Order NewOrder = new Order();
        List<Order> order = PharmacyClass.db.Order.ToList();
        NewOrder = order.Last();
        NewOrder.Order_Price = Convert.ToDecimal(OrderPrice);
        NewOrder.Order_Number += 1;
        NewOrder.User_ID = authUser.ID_User;
        PharmacyClass.db.Order.Add(NewOrder);
        try
        {
            PharmacyClass.db.SaveChanges();
        }
        catch (Exception ex) { MessageBox.Show(ex.ToString()); return; }
        medicaments.Clear();
        MessageBox.Show("Заказ успешно оформлен");
    }
}

public double TotalCost(int v1, double v2)
{
    return v1 * v2;
}

public int AddTicket(int v)
{
    v++;
    return v;
}

public int PlacesManipulation(int v1, int v2)
{
    v1 -= v2;
    return v1;
}