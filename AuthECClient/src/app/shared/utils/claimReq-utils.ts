export const claimReq = {
  adminOnly: (c: any) => c.role == 'Admin',
  adminOrTeacher: (c: any) => c.role == 'Admin' || c.role == 'Teacher',
  
}